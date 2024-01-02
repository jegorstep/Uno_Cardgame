namespace Domain;

public class Player
{

    public Guid GamePlayerId { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<Card> Hand { get; set; } = new List<Card>();
    public int Points { get; set; } = 0;
    
    public bool IsHuman { get; set;}
    
    public Player(string name, bool isHuman)
    {
        Name = name;
        this.IsHuman = isHuman;
    }

    public string GetHandInString()
    {
        string s = "";
        foreach (var card in Hand)
        {
            s += card + " | ";
        }

        return s;
    }
}