﻿namespace Domain;

[Serializable]
public class GameState
{
    public int MaxCardsInHand { set; get; } = 7;
    public int PointsToWin { get; set; } = 1000; //The first one, who scores PointsToWin points wins
    
    public bool IsReverse { get; set; } = false;
    public int IndexOfActivePlayer { get; set; }
    public int PlayerAmount { get; set; }
    public List<IPlayer> Players { get; set; } = new List<IPlayer>();
    public DeckOfCards Deck { get; set; } = new DeckOfCards();
    public Card? LastCardOnDiscardPile { get; set; }
}