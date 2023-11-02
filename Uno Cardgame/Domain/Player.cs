namespace Domain;

public interface IPlayer
{

    public string Name { get; set; }
    public List<Card> Hand { get; set; }
    public int Points { get; set; }


    public void GetHandInString();
}