namespace Domain;

public class AI
{

    public string Name { get; set; }
    public List<Card> Hand { get; set; } = new List<Card>();
    public int Points { get; set; } = 0;
    
    public AI (int id)
    {
        Name = "Player " + id;
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