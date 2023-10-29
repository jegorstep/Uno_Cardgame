namespace GameEngine;
using Players;
using Cards;

public class Game
{
    private bool _isReverse = false;
    private int _indexOfActivePlayer;
    private int _playerAmount;
    private List<Player> _players = new List<Player>();
    private DeckOfCards _deck = new DeckOfCards();
    private List<Card> _discardPile = new List<Card>();
    private Card _lastCardOnDiscardPile;
    
    public Game(int players)
    {
        _playerAmount = players;
    }

    public void Run()
    {
        _deck.SetUpDeck();
        Console.WriteLine("Deal Cards..."); 
        Thread.Sleep(2000);
        CreatePlayers();
        DealCard();
        introducePlayers();
        _lastCardOnDiscardPile = _deck.GetDeck.Pop();
       
        while (true)
        {
            Console.WriteLine("Your turn: " + _players[_indexOfActivePlayer].GetName());
            Console.Write("Card on discard pile is: ");
            Console.WriteLine(_lastCardOnDiscardPile.ToString());
            PlayerAction();
            Player player = CheckHands();
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


    private Player CheckHands()
    {
        foreach (var player in _players)
        {
            if (player.Hand.Count == 0)
            {
                Console.WriteLine(player.Name + " is a winner of this round!");
                Console.WriteLine("Counting Points...");
                Thread.Sleep(5000);
                Console.WriteLine("Dealing cards...");
                Thread.Sleep(2000);
                _deck.SetUpDeck();
                DealCard();
                return player;
            }
        }

        return null;
    }

    private bool CountPoints(Player? player)
    {
        
        if (player == null)
        {
            return false;
        }
        foreach (var opponent in _players)
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

        if (player.Points >= 500)
        {
            Console.WriteLine(player.Name.ToUpper() + " WON THE GAME, CONGRATULATIONS!");
            return true;
        }
        else
        {
            Console.WriteLine(player.Name + " has " + player.Points + " points!");
        }
        return false;
    }


    

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void PlayerAction()
    {
        if (_indexOfActivePlayer >= _playerAmount)
        {
            _indexOfActivePlayer = 0;
        }
        else if (_indexOfActivePlayer < 0)
        {
            _indexOfActivePlayer = _playerAmount - 1;
        }

        Card actionCard = null;
        Player player = _players[_indexOfActivePlayer];
        Console.Write("Your cards: ");
        player.GetHand();
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
                if (_deck.GetDeck.TryPop(out card))
                {
                    if (card.CardColor != _lastCardOnDiscardPile.CardColor && card.CardValue != _lastCardOnDiscardPile.CardValue && card.CardColor != Card.Color.Wild)
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
                    actionCard = _deck.GetDeck.Pop();
                }
                break;
            }
            Console.WriteLine();
            Console.Write(player.Name + " choose card you want to play: ");
            var chosenCardIndex = Console.ReadLine().Trim();
            var chosenIndexInt = 0;
            if (int.TryParse(chosenCardIndex, out chosenIndexInt)) // User must choose card index from 1 to ...
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
        Console.WriteLine();
        ActionManager(player, actionCard);
    }



    private void ActionManager(Player player, Card? actionCard)
    {
        if (actionCard != null)
        {
            _discardPile.Add(_lastCardOnDiscardPile);
            _lastCardOnDiscardPile = actionCard;
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
                if (_playerAmount == 2)
                {
                    SkipPlayer();
                }
                _isReverse = !_isReverse;
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


    private void RefreshDeckOfCards()
    {
        List<Card> cardsRemainedInGame = new List<Card>(); // action card + all players cards
        foreach (var player in _players)
        {
            foreach (var card in player.Hand)
            {
                cardsRemainedInGame.Add(card);
            }
        }
        cardsRemainedInGame.Add(_lastCardOnDiscardPile);
        _deck.SetUpDeck();

        Card cardForNewStack;
        Stack<Card> newStack = new Stack<Card>();
        while (_deck.GetDeck.TryPeek(out cardForNewStack))
        {
            if (!cardsRemainedInGame.Contains(cardForNewStack))
            {
                newStack.Push(cardForNewStack);
            }
        }

        _deck.GetDeck = newStack;
    }


    private void Wild(Card actionCard)
    {
        while (true)
        {
            Console.WriteLine("y) Yellow");
            Console.WriteLine("g) Green");
            Console.WriteLine("b) Blue");
            Console.WriteLine("r) Red");
            Console.Write("Choose color: ");
            var input = Console.ReadLine().Trim();
            Console.WriteLine();
            if (input.ToLower() == "y")
            {
                actionCard.CardColor = Card.Color.Yellow;
                break;
            }
            else if (input.ToLower() == "g")
            {
                actionCard.CardColor = Card.Color.Green;
                break;
            }
            else if (input.ToLower() == "b")
            {
                actionCard.CardColor = Card.Color.Blue;
                break;
            }
            else if (input.ToLower() == "r")
            {
                actionCard.CardColor = Card.Color.Red;
                break;
            }
            else
            {
                Console.WriteLine("Try again, sucker!");
            }
        }
    }


    private void DrawTwo()
    {
        Card card1 = null;
        if (_deck.GetDeck.TryPop(out card1))
        {
            _players[_indexOfActivePlayer].Hand.Add(card1);
        }
        Card card2 = null;
        if (_deck.GetDeck.TryPop(out card2))
        {
            _players[_indexOfActivePlayer].Hand.Add(card2);
        }
    }

    private void SkipPlayer()
    {
        if (_isReverse)
        {
            _indexOfActivePlayer--;
        }
        else
        {
            _indexOfActivePlayer++;
        }
        
        if (_indexOfActivePlayer >= _playerAmount)
        {
            _indexOfActivePlayer = 0;
        }
        else if (_indexOfActivePlayer < 0)
        {
            _indexOfActivePlayer = _playerAmount - 1;
        }
    }

    private List<Card> PossibleMoves(List<Card> playerHand)
    {
        List<Card> possibleCardsToPlay = new List<Card>();
        List<Card> WildDrawFourCards = new List<Card>();
        foreach (var card in playerHand)
        {
            if (card.CardColor.Equals(_lastCardOnDiscardPile.CardColor))
            {
                possibleCardsToPlay.Add(card);
            }
            else if (card.CardValue.Equals(_lastCardOnDiscardPile.CardValue))
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
        foreach (var gamePlayer in _players)
        {
            for (int j = 0; j != 7; j++)
            {
                gamePlayer.Hand.Add(_deck.GetDeck.Pop());
            }
        }
    }

    private List<Player> CreatePlayers()
    {
        for (int i = 0; i < _playerAmount; i++)
        {
            Console.WriteLine();
            Console.Write("Type player name for " + (i + 1) + " player: ");
            var newPlayerName = Console.ReadLine().Trim();
            _players.Add(new Player(newPlayerName));
        }

        return _players;
    }

    private void introducePlayers()
    {
        Console.Clear();
        Random rand = new Random();
        var chosenPlayer = 0;
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Who plays first?");
            var x = 1;
            Console.WriteLine("r) Random");
            foreach (var player in _players)
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
                chosenPlayer = rand.Next(_playerAmount);
                _indexOfActivePlayer = chosenPlayer;
                break;
            }
            else if (int.TryParse(input, out chosenPlayer) && chosenPlayer >= 0 && chosenPlayer <= _playerAmount)
            {
                _indexOfActivePlayer = chosenPlayer - 1;
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("You need to type number of a player or type r so gods of random choose for you!");
            }
        }
    }
}