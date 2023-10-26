using Cards;

namespace Players;

public class Player
{

    public bool IsActive = false;
    public string Name;
    public List<Card> Hand = new List<Card>();
    
    public Player(int id)
    {
        Name = "Player " + id;
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