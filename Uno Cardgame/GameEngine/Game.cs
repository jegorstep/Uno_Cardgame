using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace GameEngine;


public class Game
{
    private IGameRepository _saveSystem = default!;
    private GameState _gameState = new GameState();
    private bool _exitGame;
    private bool _isNewGame = true;
    private Random _random = new Random();
    private GameUI _gameUi = new GameUI();

    public Game(int players, string gameType)
    {
        _gameState.PlayerAmount = players;
        if (gameType.Equals("Custom"))
        {
            string[] options = { "Yes", "No" };
            int answer = _gameUi.UniversalMenu("Do you want short game? (one round)", options);
            if (answer == 0)
            {
                _gameState.ShortGame = true;
            }

            string[] maxAmountOfCards = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            answer = _gameUi.UniversalMenu("How many cards will be in your hand?", maxAmountOfCards);
            _gameState.MaxCardsInHand = answer + 1;

            answer = _gameUi.UniversalMenu(
                "Do you want add 'swapping seven' rule? \n(You can choose player with whom you can swap cards)",
                options);
            if (answer == 0)
            {
                _gameState.SwappingCards = true;
            }
        }
    }



    public Game(GameState gameState)
    {
        _gameState = gameState;
        _isNewGame = false;
    }

    public void Run()
    {
        if (_isNewGame)
        {
            SetUpGame();
        }
            
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Your turn: " + _gameState.Players[_gameState.IndexOfActivePlayer].Name);
            Thread.Sleep(2000);
            Console.WriteLine(_gameState.Players[_gameState.IndexOfActivePlayer].Name + " has " +
                               _gameState.Players[_gameState.IndexOfActivePlayer].Hand.Count + " cards");
            Console.WriteLine("Card on discard pile is: " + _gameState.LastCardOnDiscardPile!);
            Thread.Sleep(2000);
            PlayerAction();
            Console.WriteLine();
            if (_exitGame)
            {
                _exitGame = false;
                break;
            }
            Player player = CheckHands();
            if (player != null && _gameState.ShortGame)
            {
                Console.WriteLine();
                Console.WriteLine(player.Name.ToUpper() + " WON THE GAME, CONGRATULATIONS!");
                Thread.Sleep(4000);
                break;
            }
            
            if (CountPoints(player))
            {
                _gameUi.PrintEndGame();
                Thread.Sleep(2000);
                break;   
            }
        }
    }
    
    public void PlayerAction()
    {
        Card actionCard = default!;
        if (_gameState.IndexOfActivePlayer >= _gameState.PlayerAmount)
        {
            _gameState.IndexOfActivePlayer = 0;
        }
        else if (_gameState.IndexOfActivePlayer < 0)
        {
            _gameState.IndexOfActivePlayer = _gameState.PlayerAmount - 1;
        }
        
        Player player = _gameState.Players[_gameState.IndexOfActivePlayer];
        
        if (!player.IsHuman)
        {
            ActionManager(player, AiChooseCard(player));
        }
        else
        {
            
            Console.WriteLine("\n(Press any KEY to continue)");
            Console.ReadKey(true);
            while (true)
            {
                string title = "Previously played card: " + _gameState.LastCardOnDiscardPile + "\n" +
                               "Your cards: " + player.GetHandInString() + "\n" +
                               "Choose card to play";


                Console.WriteLine();
                List<Card> possibleMoves = PossibleMoves(player.Hand);
                if (possibleMoves.Count == 0)
                {
                    actionCard = NoPossibleMoves(player)!;
                    break;
                }
                else
                {
                    var answer = _gameUi.DisplayCards(title, possibleMoves);
                    if (answer == possibleMoves.Count)
                    {
                        SaveGame();
                    }
                    else if (answer == possibleMoves.Count + 1)
                    {
                        _exitGame = true;
                        break;
                    }
                    else
                    {
                        actionCard = possibleMoves[answer];
                        break;
                    }
                }
                Console.Clear();
            }
            ActionManager(player, actionCard);
        }

        Console.WriteLine();
        
        
    }


    private Card? AiChooseCard(Player player)
    {
        Console.WriteLine();
        List<Card> possibleMoves = PossibleMoves(player.Hand);
        if (possibleMoves.Count == 0)
        {
            return NoPossibleMoves(player);
        }
        
        return possibleMoves[_random.Next(possibleMoves.Count)];
    }


    private Card? NoPossibleMoves(Player player)
    {
        if (_gameState.Deck.GetDeck.TryPop(out var card))
        {
            Console.WriteLine("It seems " + player.Name + " don't have moves, take card...");
            Thread.Sleep(2000);
            if (card.CardColor != _gameState.LastCardOnDiscardPile!.CardColor
                && card.CardValue != _gameState.LastCardOnDiscardPile.CardValue &&
                card.CardColor != Card.Color.Wild)
            {
                Console.WriteLine("Card is not playable, skip turn, " + player.Name + " sucks!");
                Thread.Sleep(2000);
                player.Hand.Add(card);
                return null;
            }
            
            Console.WriteLine("Seems you found right card, you play card " + card);
            Thread.Sleep(2000);
            return card;
            
        }
        else
        {
            Console.WriteLine("Deck is empty, refresh it");
            Thread.Sleep(2000);
            RefreshDeckOfCards();
            card = NoPossibleMoves(player)!;
        }

        return card;

    }



    public void ActionManager(Player player, Card? actionCard)
    {
        if (actionCard != null)
        {
            _gameState.LastCardOnDiscardPile = actionCard;
            player.Hand.Remove(actionCard);
            if (actionCard.CardValue == Card.Value.Skip)
            {
                SkipPlayer();
            }
            else if (actionCard.CardValue == Card.Value.Reverse)
            {
                if (_gameState.PlayerAmount == 2)
                {
                    SkipPlayer();
                }
                _gameState.IsReverse = !_gameState.IsReverse;
            }
            else if (actionCard.CardValue == Card.Value.DrawTwo)
            {
                SkipPlayer(); // skip player, because he takes two cards and skip turn
                DrawTwo();
            }
            else if (actionCard.CardValue == Card.Value.Wild)
            {
                Wild(actionCard);
            }
            else if (actionCard.CardValue == Card.Value.WildDrawFour)
            {
                Wild(actionCard);
                SkipPlayer();
                DrawTwo();
                DrawTwo();
            }
            else if (actionCard.CardValue == Card.Value.Seven && _gameState.SwappingCards)
            {
                List<string> playersNames = new List<string>();
                List<Player> players = new List<Player>();
                
                foreach (var igrok in _gameState.Players)
                {
                    if (!player.Equals(igrok))
                    {
                        playersNames.Add(igrok.Name);
                        players.Add(igrok);
                    }
                }
                int indexOfPlayer = _gameUi.UniversalMenu("Choose with whom you want to swap cards",playersNames.ToArray());
                Player playerToSwapCards = players[indexOfPlayer];
                (player.Hand, playerToSwapCards.Hand) = (playerToSwapCards.Hand, player.Hand);
            }
            Console.WriteLine(player.Name + " plays card " + actionCard);
            Thread.Sleep(2000);
        }
        
        if (player.Hand.Count == 1)
        {
            Console.WriteLine(player.Name + " SAYS  U N O!");
            Thread.Sleep(2000);
        }
        SkipPlayer();
    }

    private void RefreshDeckOfCards()
    {
        List<Card> cardsRemainedInGame = new List<Card>(); // action card + all players cards
        foreach (var player in _gameState.Players)
        {
            foreach (var card in player.Hand)
            {
                cardsRemainedInGame.Add(card);
            }
        }

        cardsRemainedInGame.Add(_gameState.LastCardOnDiscardPile!);
        _gameState.Deck.SetUpDeck();

        Stack<Card> newStack = new Stack<Card>();
        while (_gameState.Deck.GetDeck.TryPeek(out var cardForNewStack))
        {
            if (!cardsRemainedInGame.Contains(cardForNewStack))
            {
                newStack.Push(cardForNewStack);
            }
        }

        _gameState.Deck.GetDeck = newStack;
    }


    private void Wild(Card actionCard)  
    {
       
        var player = _gameState.Players[_gameState.IndexOfActivePlayer];
        int input;
        string[] colors = {"Yellow", "Blue", "Green", "Red"};
        Card.Color[] eColors = { Card.Color.Yellow, Card.Color.Blue, Card.Color.Green, Card.Color.Red };
        if (!player.IsHuman)
        {
            input = _random.Next(colors.Length);
            Console.WriteLine(player.Name + " chooses color " + colors[input]);
            actionCard.CardColor = eColors[input];
            Thread.Sleep(2000);
        }
        else
        {
            input = _gameUi.UniversalMenu("Choose color", colors);
            actionCard.CardColor = eColors[input];
        }
        
    }

    public void WildForWeb(Card card, string color)
    {
        var player = _gameState.Players[_gameState.IndexOfActivePlayer];
        
        foreach (Card.Color colorInEnum in Enum.GetValues(typeof(Card.Color)))
        {
            if (color.Equals(colorInEnum.ToString()))
            {
                player.Hand.Remove(card);
                card.CardColor = colorInEnum;
                _gameState.LastCardOnDiscardPile = card;
                break;
            }
        }
        if (card.CardValue == Card.Value.WildDrawFour)
        {
            SkipPlayer();
            DrawTwo();
            DrawTwo();
        }
        SkipPlayer();
    }


    private void DrawTwo()
    {
        if (_gameState.Deck.GetDeck.TryPop(out var card1))
        {
            _gameState.Players[_gameState.IndexOfActivePlayer].Hand.Add(card1);
        }

        if (_gameState.Deck.GetDeck.TryPop(out var card2))
        {
            _gameState.Players[_gameState.IndexOfActivePlayer].Hand.Add(card2);
        }
    }

    public void SaveGame()
    {
        // if web application, using database
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=" + Path.Combine(Path.GetTempPath(), "savedGamesDb"))
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;
        using var db = new AppDbContext(contextOptions);
        db.Database.Migrate();
        _saveSystem = new GameRepositoryEF(db);
        // end of database code
            
            
            
        // if file save system
        // _saveSystem = new GameRepositoryFileSystem();
        // end of file save system
        
        
        
        Console.WriteLine();
        _saveSystem.Save(_gameState.Id, _gameState);
        Thread.Sleep(1000);
        Console.WriteLine("Game have been saved successfully!");
    }

    private void SkipPlayer()
    {
        if (_gameState.IsReverse)
        {
            _gameState.IndexOfActivePlayer--;
        }
        else
        {
            _gameState.IndexOfActivePlayer++;
        }
        
        if (_gameState.IndexOfActivePlayer >= _gameState.PlayerAmount)
        {
            _gameState.IndexOfActivePlayer = 0;
        }
        else if (_gameState.IndexOfActivePlayer < 0)
        {
            _gameState.IndexOfActivePlayer = _gameState.PlayerAmount - 1;
        }
    }

    public List<Card> PossibleMoves(List<Card> playerHand)
    {
        List<Card> possibleCardsToPlay = new List<Card>();
        List<Card> wildDrawFourCards = new List<Card>();
        foreach (var card in playerHand)
        {
            if (card.CardColor.Equals(_gameState.LastCardOnDiscardPile!.CardColor))
            {
                possibleCardsToPlay.Add(card);
            }
            else if (card.CardValue.Equals(_gameState.LastCardOnDiscardPile.CardValue))
            {
                possibleCardsToPlay.Add(card);
            }
            else if (card.CardValue == Card.Value.Wild)
            {
                possibleCardsToPlay.Add(card);
            }
            else if (card.CardValue == Card.Value.WildDrawFour)
            {
                wildDrawFourCards.Add(card);
            }
        }

        if (possibleCardsToPlay.Count == 0)
        {
            possibleCardsToPlay = wildDrawFourCards;
        }
        
        return possibleCardsToPlay;
    }
    
    
    private void DealCard()
    {
        foreach (var gamePlayer in _gameState.Players)
        {
            gamePlayer.Hand.Clear();
            for (int j = 0; j != _gameState.MaxCardsInHand; j++)
            {
                gamePlayer.Hand.Add(_gameState.Deck.GetDeck.Pop());
            }
        }
    }

    private void CreatePlayers()
    {
        string[] options = new string[_gameState.PlayerAmount + 1];
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = i + "";
        }

        int aiAmount = _gameUi.UniversalMenu("Choose amount of AI", options);
        
        for (int i = 1; i <= aiAmount; i++)
        {
            Player robot = new Player("Player " + i, false);
            _gameState.Players.Add(robot);
        }

        
        for (int i = 1; i <= _gameState.PlayerAmount - aiAmount; i++)
        {
            Console.Write("Type name for " + i + " human player: ");
            var newPlayerName = Console.ReadLine()!.Trim();
            Player player = new Player(newPlayerName, true);
            _gameState.Players.Add(player);
            Console.WriteLine();
        }
    }

    private void IntroducePlayers()
    {
        string[] players = new string[_gameState.PlayerAmount + 1];
        int i = 0;
        foreach (var player in _gameState.Players)
        {
            players[i] = player.Name;
            i++;
        }
        players[i] = "Random";

        var input = _gameUi.UniversalMenu("Choose who play first", players);
        if (input == i)
        {
            input = _random.Next(_gameState.PlayerAmount);
            _gameState.IndexOfActivePlayer = input;
        }
        _gameState.IndexOfActivePlayer = input;
    }
    
    
    private void SetUpGame()
    {
        _gameState.Deck.SetUpDeck();
        _gameUi.PrintDealCards();
        CreatePlayers();
        while (true) // deal with unwanted wild card as lastCardOnDiscardPile
        {
            _gameState.LastCardOnDiscardPile = _gameState.Deck.GetDeck.Peek();
            if (_gameState.LastCardOnDiscardPile.CardColor != Card.Color.Wild)
            {
                _gameState.LastCardOnDiscardPile = _gameState.Deck.GetDeck.Pop();
                break;
            }

            _gameState.Deck.SetUpDeck();
        }

        DealCard();
        IntroducePlayers();
    }


    private Player CheckHands()
    {
        foreach (var player in _gameState.Players)
        {
            if (player.Hand.Count == 0)
            {
                _gameUi.PrintWinnerOfRound(player.Name, _gameState.ShortGame);
                if (!_gameState.ShortGame)
                { 
                    _gameState.Deck.SetUpDeck();
                    DealCard();
                }
                return player;
            }
        }

        return null!;
    }

    private bool CountPoints(Player? player)
    {
        
        if (player == null)
        {
            return false;
        }
        foreach (var opponent in _gameState.Players)
        {
            if (!player.Equals(opponent))
            {
                foreach (var card in opponent.Hand)
                {
                    switch (card.CardValue)
                    {
                        case Card.Value.WildDrawFour:
                            player.Points += 50;
                            break;
                        case Card.Value.Wild:
                            player.Points += 50;
                            break;
                        case Card.Value.DrawTwo:
                            player.Points += 20;
                            break;
                        case Card.Value.Skip:
                            player.Points += 20;
                            break;
                        case Card.Value.Reverse:
                            player.Points += 20;
                            break;
                        default:
                            player.Points += (int)card.CardValue;
                            break;
                    }
                }
            }
        }

        if (player.Points >= _gameState.PointsToWin)
        {
            Console.WriteLine(player.Name.ToUpper() + " WON THE GAME, CONGRATULATIONS!");
            return true;
        }

        _gameUi.PrintWinnerPoints(player);
        
        return false;
    }
    
}