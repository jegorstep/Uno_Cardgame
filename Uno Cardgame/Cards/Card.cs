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
        string colorStr = ConvertColorEnumToString((int) CardColor);
        string valueStr = Enum.GetName(typeof(Value), CardValue)!;

        return $"{colorStr}{valueStr}";
    }
    
    public string ConvertColorEnumToString(int colorValue)
    {
        switch (colorValue)
        {
            case (int)Color.Yellow:
                return "🟨";
            case (int)Color.Green:
                return "🟩";
            case (int)Color.Red:
                return "🟥";
            case (int)Color.Blue:
                return "🟦";
            case (int)Color.Wild:
                return "⬛️";
            // Add more cases for other enum values as needed
            default:
                return "-";
        }
    }
}