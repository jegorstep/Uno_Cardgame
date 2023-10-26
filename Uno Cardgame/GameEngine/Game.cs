namespace GameEngine;
using Players;
using Cards;



public class Game
{
    private int _playerAmount = 0;
    private List<Player> _players = new List<Player>();
    private DeckOfCards _deck = new DeckOfCards();
    
    public Game(int players)
    {
        _playerAmount = players;
    }

    public void Run()
    {
        _deck.SetUpDeck();
        Console.WriteLine("Deal Cards..."); 
        Thread.Sleep(2000);
        DealCard();
        introducePlayers();
        
        
    }
    
    
    
    
    
    
    
    
    
    
    
    
    private void DealCard()
    {
        for (int i = 0; i != _playerAmount; i++)
        {
            _players.Add(new Player(i + 1));
            for (int j = 0; j != 7; j++)
            {
                _players[i].Hand.Add(_deck.GetDeck.Pop());
            }
        }
    }

    private List<Player> CreatePlayers()
    {
        for (int i = 0; i < _playerAmount; i++)
        {
            _players.Add(new Player(i + 1));
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
            Console.WriteLine("Who plays first?");
            var x = 1;
            Console.WriteLine("r) Random");
            foreach (var player in _players)
            {
                Console.Write(x + ") ");
                x++;
                Console.WriteLine(player.Name);
            }

            var input = Console.ReadLine().Trim();
            if (input.Equals("r"))
            {
                chosenPlayer = rand.Next(_playerAmount - 1);
                _players[chosenPlayer].IsActive = true;
                break;
            }
            else if (int.TryParse(input, out chosenPlayer) && chosenPlayer >= 0 && chosenPlayer <= _playerAmount)
            {
                _players[chosenPlayer].IsActive = true;
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