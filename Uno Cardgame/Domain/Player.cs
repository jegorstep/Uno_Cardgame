namespace Domain;

public class Player
{

    public string Name { get; set; }
    public List<Card> Hand { get; set; } = new List<Card>();
    public int Points { get; set; } = 0;
    
    public bool isHuman { get; set;}
    
    public Player(string name, bool isHuman)
    {
        Name = name;
        this.isHuman = isHuman;
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