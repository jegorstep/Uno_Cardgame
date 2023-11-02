

namespace Domain;

public class Human : IPlayer
{

    public string Name { get; set; }
    public List<Card> Hand { get; set; } = new List<Card>();
    public int Points { get; set; } = 0;
    
    public Human(string name)
    {
        Name = name;
    }

    public void GetHandInString()
    {
        foreach (var card in Hand)
        {
            Console.Write(card);
            Console.Write(" | ");
        }
    }
}