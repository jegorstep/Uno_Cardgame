namespace Domain;

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
                return "Yellow ";
            case (int)Color.Green:
                return "Green ";
            case (int)Color.Red:
                return "Red ";
            case (int)Color.Blue:
                return "Blue ";
            case (int)Color.Wild:
                return "Wild ";
            // Add more cases for other enum values as needed
            default:
                return "-";
        }
    }

    public string IntValue(Value value)
    {
        if ((int)value == 10)
        {
            return "S";
        }
        else if ((int)value == 11)
        {
            return "R";
        }
        if ((int)value == 12)
        {
            return "DT";
        }
        else if ((int)value == 13)
        {
            return "W";
        }
        else if ((int)value == 14)
        {
            return "WD4";
        }
        return ((int)value).ToString();
    }
    public int IntColor(Color color)
    {
        return (int)color;
    }
}