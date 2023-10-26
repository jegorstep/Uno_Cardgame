namespace Cards;

public class Card 
{
    public enum Color
    {
        Yellow,
        Green,
        Red,
        Blue,
        Wild
    }

    public enum Value
    {
        Zero, One, Two, Three, Four, Five, Six, Seven,
        Eight, Nine, Skip, Reverse, DrawTwo, Wild, WildDrawFour
    }
    
    
    //properties
    public Color CardColor { get; set; }
    public Value CardValue { get; set; }

    public override string ToString()
    {
        string colorStr = Enum.GetName(typeof(Color), CardColor)!;
        string valueStr = Enum.GetName(typeof(Value), CardValue)!;

        return $"{colorStr} {valueStr}";
    }
}