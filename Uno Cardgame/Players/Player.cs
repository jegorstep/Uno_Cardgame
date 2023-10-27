using Cards;

namespace Players;

public class Player
{

    public string Name;
    public List<Card> Hand = new List<Card>();
    public int Points = 0;
    
    public Player(string name)
    {
        Name = name;
    }

    public void getHand()
    {
        foreach (var card in Hand)
        {
            Console.Write(card);
            Console.Write(" | ");
        }
    }
}