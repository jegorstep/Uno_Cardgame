namespace Cards;

public class DeckOfCards : Card
{
    private const int NumberOfCards = 108;
    public Stack<Card> GetDeck { get; } = new Stack<Card>();
    
    
    
    public void SetUpDeck()
    {
        List<Card> deck = new List<Card>();
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            if (color.Equals(Color.Wild))
            {
                continue;
            }
            foreach (Value value in Enum.GetValues(typeof(Value)))
            {
                if (value.Equals(Value.Wild) || value.Equals(Value.WildDrawFour))
                {
                    continue;   
                }
                if (!value.Equals(Value.Zero))
                {
                    deck.Add(new Card { CardColor = color, CardValue = value });
                }
                deck.Add(new Card { CardColor = color, CardValue = value });
            }
        }

        for (int i = 0; i < 4; i++)
        {
            deck.Add(new Card {CardColor = Color.Wild, CardValue = Value.Wild});
            deck.Add(new Card {CardColor = Color.Wild, CardValue = Value.WildDrawFour});
        }
        
        
        ShuffleCards(deck);
        foreach (var card in deck)
        {
         GetDeck.Push(card);   
        }
    }

    //shuffle the deck
    private void ShuffleCards(List<Card> deck)
    {
        var rand = new Random();
        Card temp;
        
        //run the shuffle 1000 times
        for (int shuffleTimes = 0; shuffleTimes < 1000; shuffleTimes++)
        {
            for (int i = 0; i < NumberOfCards; i++)
            {
                //swap the cards
                var secondCardIndex = rand.Next(NumberOfCards - i); //One color cards
                temp = deck[i];
                deck[i] = deck[secondCardIndex];
                deck[secondCardIndex] = temp;
            }
        }
    }

}