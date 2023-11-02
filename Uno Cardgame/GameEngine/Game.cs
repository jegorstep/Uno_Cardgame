using System.Net;
using Domain;
using DAL;
namespace GameEngine;


public class Game
{
    private GameRepositoryFileSystem _saveSystem = new GameRepositoryFileSystem();
    private GameState _gameState = new GameState();
    private bool exitGame = false;
    private bool _isNewGame = true;
    private Random _random = new Random();
    
    public Game(int players)
    {
        _gameState.PlayerAmount = players;
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
            _gameState.Deck.SetUpDeck();
            Console.Clear();
            Console.Write("Deal Cards");
            Thread.Sleep(800);
            Console.Write(".");
            Thread.Sleep(800);
            Console.Write(".");
            Thread.Sleep(800);
            Console.Write(".");
            Thread.Sleep(800);
            Console.Clear();
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
            introducePlayers();
        }

        Console.Clear();
        while (true)
        {
            Console.WriteLine("Your turn: " + _gameState.Players[_gameState.IndexOfActivePlayer].Name);
            Console.Write("Card on discard pile is: ");
            Console.WriteLine(_gameState.LastCardOnDiscardPile.ToString());
            PlayerAction();
            Console.WriteLine();
            if (exitGame)
            {
                exitGame = false;
                break;
            }
            IPlayer player = CheckHands();
            if (CountPoints(player))
            {
                Console.WriteLine("See ya, suckers!");
                Console.WriteLine();
                Console.WriteLine(">> T H E  E N D <<");
                Console.WriteLine();
                Thread.Sleep(2000);
                break;   
            }
        }
    }


    private IPlayer CheckHands()
    {
        foreach (var player in _gameState.Players)
        {
            if (player.Hand.Count == 0)
            {
                Console.WriteLine(player.Name + " is a winner of this round!");
                Console.WriteLine();
                Console.WriteLine("Counting Points...");
                Thread.Sleep(5000);
                Console.WriteLine("Dealing cards...");
                Thread.Sleep(2000);
                Console.WriteLine();
                _gameState.Deck.SetUpDeck();
                DealCard();
                return player;
            }
        }

        return null;
    }

    private bool CountPoints(IPlayer? player)
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
                        case Card.Value.Wild:
                            player.Points += 50;
                            break;
                        case Card.Value.DrawTwo:
                        case Card.Value.Skip:
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
        else
        {
            Console.WriteLine();
            Console.WriteLine(player.Name + " has now " + player.Points + " points!");
            Console.WriteLine();
        }
        return false;
    }


    

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void PlayerAction()
    {
        if (_gameState.IndexOfActivePlayer >= _gameState.PlayerAmount)
        {
            _gameState.IndexOfActivePlayer = 0;
        }
        else if (_gameState.IndexOfActivePlayer < 0)
        {
            _gameState.IndexOfActivePlayer = _gameState.PlayerAmount - 1;
        }

        Card actionCard = null;
        IPlayer player = _gameState.Players[_gameState.IndexOfActivePlayer];
        if (player is AI)
        {
            Console.Write(player.Name + " cards: ");
            player.GetHandInString();
            Console.WriteLine();
            List<Card> possibleMoves = PossibleMoves(player.Hand);
            if (possibleMoves.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("It seems " + player.Name + " don't have moves, he takes card...");
                Thread.Sleep(2000);
                Card card;
                if (_gameState.Deck.GetDeck.TryPop(out card))
                {
                    if (card.CardColor != _gameState.LastCardOnDiscardPile.CardColor
                        && card.CardValue != _gameState.LastCardOnDiscardPile.CardValue &&
                        card.CardColor != Card.Color.Wild)
                    {
                        Console.WriteLine("Card is not playable, skip turn, " + player.Name + " sucks!");
                        Thread.Sleep(2000);
                        player.Hand.Add(card);
                    }
                    else
                    {
                        Console.WriteLine("Seems " + player.Name + " found right card, he plays card " + card);
                        Thread.Sleep(2000);
                        actionCard = card;
                    }
                }
                else
                {
                    RefreshDeckOfCards();
                    actionCard = _gameState.Deck.GetDeck.Pop();
                }
            }
            else
            {
                actionCard = possibleMoves[_random.Next(possibleMoves.Count())];
            }
        }
        else
        {
            Console.Write("Your cards: ");
            player.GetHandInString();
            while (actionCard == null) // force player to choose right card
            {
                Console.WriteLine();
                Console.Write("Possible moves: ");
                List<Card> possibleMoves = PossibleMoves(player.Hand);
                if (possibleMoves.Count == 0)
                {
                    Console.WriteLine("It seems you don't have moves, you take card...");
                    Thread.Sleep(2000);
                    Card card;
                    if (_gameState.Deck.GetDeck.TryPop(out card))
                    {
                        if (card.CardColor != _gameState.LastCardOnDiscardPile.CardColor
                            && card.CardValue != _gameState.LastCardOnDiscardPile.CardValue &&
                            card.CardColor != Card.Color.Wild)
                        {
                            Console.WriteLine("Card is not playable, you skip turn, " + player.Name + " sucks!");
                            Thread.Sleep(2000);
                            player.Hand.Add(card);
                        }
                        else
                        {
                            Console.WriteLine("Seems you found right card, you play card " + card);
                            Thread.Sleep(2000);
                            actionCard = card;
                        }
                    }
                    else
                    {
                        RefreshDeckOfCards();
                        actionCard = _gameState.Deck.GetDeck.Pop();
                    }

                    break;
                }

                Console.WriteLine();
                Console.WriteLine("s) Save Game");
                Console.WriteLine("x) Exit Game");
                Console.Write(player.Name + " choose card you want to play: ");

                var chosenIndexInt = 0;
                var chosenCardIndex = Console.ReadLine().Trim();
                if (chosenCardIndex.ToLower() == "s")
                {
                    saveGame();
                }
                else if (chosenCardIndex.ToLower() == "x")
                {
                    exitGame = true;
                    break;
                }
                else if (int.TryParse(chosenCardIndex, out chosenIndexInt)) // User must choose card index from 1 to ...
                {
                    if (chosenIndexInt >= 1 && chosenIndexInt <= possibleMoves.Count)
                    {
                        actionCard = possibleMoves[chosenIndexInt - 1]; // index decrease by 1
                    }
                }
                else
                {
                    Console.WriteLine("Try again, loser!");
                }
            }
        }

        Console.WriteLine();
        
        ActionManager(player, actionCard);
        
    }



    private void ActionManager(IPlayer player, Card? actionCard)
    {
        if (actionCard != null)
        {
            _gameState.LastCardOnDiscardPile = actionCard;
            player.Hand.Remove(actionCard);
            if (player.Hand.Count == 1)
            {
                Console.WriteLine(player.Name + " SAYS  U N O!");
            }
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
        }
        SkipPlayer();
    }

    private void AIManager()
    {
        
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

        cardsRemainedInGame.Add(_gameState.LastCardOnDiscardPile);
        _gameState.Deck.SetUpDeck();

        Card cardForNewStack;
        Stack<Card> newStack = new Stack<Card>();
        while (_gameState.Deck.GetDeck.TryPeek(out cardForNewStack))
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
        while (true)
        {
            var player = _gameState.Players[_gameState.IndexOfActivePlayer];
            String input;
            string[] colors = {"y", "b", "g", "r"};
            if (player is AI)
            {
                Console.Write(player.Name + " chooses color...");
                input = colors[_random.Next(colors.Length)];

            }
            else
            {
                Console.WriteLine("y) Yellow");
                Console.WriteLine("g) Green");
                Console.WriteLine("b) Blue");
                Console.WriteLine("r) Red");
                Console.Write("Choose color: ");
                input = Console.ReadLine().Trim();
            }

            Console.WriteLine();
            if (input.ToLower() == "y")
            {
                actionCard.CardColor = Card.Color.Yellow;
                break;
            }
            if (input.ToLower() == "g")
            {
                actionCard.CardColor = Card.Color.Green;
                break;
            }
            if (input.ToLower() == "b")
            {
                actionCard.CardColor = Card.Color.Blue;
                break;
            }
            if (input.ToLower() == "r")
            {
                actionCard.CardColor = Card.Color.Red;
                break;
            }
            Console.WriteLine("Try again, sucker!");
        }
    }


    private void DrawTwo()
    {
        Card card1 = null;
        if (_gameState.Deck.GetDeck.TryPop(out card1))
        {
            _gameState.Players[_gameState.IndexOfActivePlayer].Hand.Add(card1);
        }
        Card card2 = null;
        if (_gameState.Deck.GetDeck.TryPop(out card2))
        {
            _gameState.Players[_gameState.IndexOfActivePlayer].Hand.Add(card2);
        }
    }

    private void saveGame()
    {
        Console.Write("Choose name of save: ");
        var saveName = Console.ReadLine().Trim();
        Console.WriteLine();
        var fileName = _saveSystem.SaveGame(saveName, _gameState);
        var path = Path.Combine(Path.GetTempPath(), "savedGames");
        StreamWriter writer = new StreamWriter(Path.Combine(Path.GetTempPath(), "saved_games.txt"), true);
        writer.WriteLine(fileName);
        writer.Close();
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

    private List<Card> PossibleMoves(List<Card> playerHand)
    {
        List<Card> possibleCardsToPlay = new List<Card>();
        List<Card> WildDrawFourCards = new List<Card>();
        foreach (var card in playerHand)
        {
            if (card.CardColor.Equals(_gameState.LastCardOnDiscardPile.CardColor))
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
                WildDrawFourCards.Add(card);
            }
        }

        if (possibleCardsToPlay.Count == 0)
        {
            possibleCardsToPlay = WildDrawFourCards;
        }

        var x = 1;
        foreach (var card in possibleCardsToPlay)
        {
            Console.Write(x + ") " + card +" | ");
            x++;
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

    private List<IPlayer> CreatePlayers()
    {
        int aiAsInt;
        while (true)
        {
            Console.Write("Choose amount of AI: ");
            var ai = Console.ReadLine().Trim();
            if (int.TryParse(ai, out aiAsInt))
            {
                if (aiAsInt < _gameState.PlayerAmount && aiAsInt >= 0)
                {
                    for (int i = 1; i <= aiAsInt; i++)
                    {
                        _gameState.Players.Add(new AI(i));
                    }
                    break;
                }
            }
            Console.WriteLine("Wrong input!");
        }

        for (int i = 1; i <= _gameState.PlayerAmount - aiAsInt; i++)
        {
            Console.Write("Type name for " + i + " human player: ");
            var newPlayerName = Console.ReadLine().Trim();
            _gameState.Players.Add(new Human(newPlayerName));
            Console.WriteLine();
        }
        

        return _gameState.Players;
    }

    private void introducePlayers()
    {
        Console.Clear();
        var chosenPlayer = 0;
        while (true)
        {
            Console.WriteLine("Who plays first?");
            var x = 1;
            Console.WriteLine("r) Random");
            foreach (var player in _gameState.Players)
            {
                Console.Write(x + ") ");
                x++;
                Console.WriteLine(player.Name);
            }

            Console.Write("Answer: ");
            var input = Console.ReadLine().Trim();
            Console.WriteLine();
            if (input.Equals("r"))
            {
                chosenPlayer = _random.Next(_gameState.PlayerAmount);
                _gameState.IndexOfActivePlayer = chosenPlayer;
                break;
            }
            if (int.TryParse(input, out chosenPlayer) && chosenPlayer >= 0 && chosenPlayer <= _gameState.PlayerAmount)
            {
                _gameState.IndexOfActivePlayer = chosenPlayer - 1;
                break;
            }
            Console.Clear();
            Console.WriteLine("You need to type number of a player or type r so gods of random choose for you!");
        }
    }
}