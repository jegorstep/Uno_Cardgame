namespace Domain;

[Serializable]
public class GameState
{
    public Guid Id { set; get; } = Guid.NewGuid();
    public int MaxCardsInHand { set; get; } = 7;
    public int PointsToWin { get; set; } = 500; //The first one, who scores PointsToWin points wins
    
    public bool IsReverse { get; set; } = false;
    public int IndexOfActivePlayer { get; set; } = default!;
    public int PlayerAmount { get; set; } = default!;
    public List<Player> Players { get; set; } = new List<Player>();
    public DeckOfCards Deck { get; set; } = new DeckOfCards();
    public Card? LastCardOnDiscardPile { get; set; } = default!;
    public string Log { get; set; } = default!;
    
    //Custom rules
    public bool ShortGame { get; set; } = false; 
    public bool SwappingCards { get; set; } = false;
    
    
    public string GetNameOfActivePlayer()
    {
        return Players[IndexOfActivePlayer].Name;
    }

}