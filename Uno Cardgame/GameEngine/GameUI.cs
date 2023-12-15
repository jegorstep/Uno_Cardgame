using Domain;

namespace GameEngine;

public class GameUI
{
    
    private int _selectedOption;

    private const string MenuSeparator = "=======================";

    public int UniversalMenu(string title, string[] options)
    {
        _selectedOption = 0;
        Console.CursorVisible = false;
        ConsoleKeyInfo key;
        
        do
        {
            Console.Clear();

            Console.WriteLine(title);
            Console.WriteLine(MenuSeparator);
            PrintMenu(options);
            Console.WriteLine(MenuSeparator);

            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (_selectedOption > 0)
                        _selectedOption--;
                    else _selectedOption = options.Length - 1;
                    break;
                    
                case ConsoleKey.DownArrow:
                    if (_selectedOption < options.Length - 1)
                        _selectedOption++;
                    else _selectedOption = 0;
                    break;
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.Clear();
        return _selectedOption;
    }

    public int DisplayCards(string title, List<Card> cards)
    {
        string[] cardsArr = new string[cards.Count + 2];
        int i = 0;
        foreach (var card in cards)
        {
            cardsArr[i] = card.ToString();
            i++;
        }

        cardsArr[i] = "Save";
        cardsArr[i + 1] = "Exit";
        
        return UniversalMenu(title, cardsArr);
    }

    public int DemandNumber(string query, int start, int end)
    {
        int intInput;
        Console.Write(query);
        do
        {
            var input = Console.ReadLine()!.Trim();
            int.TryParse(input, out intInput);
            if (start <= intInput && end >= intInput)
            {
                break;
            }
        } while (true);
        
        return intInput;
    }
    

    private void PrintMenu(string[] options)
    {
        for (var i = 0; i < options.Length; i++)
        {
            if (i == _selectedOption)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine($" {options[i]} ");

            Console.ResetColor();
        }
    }
    
    
    public void PrintDealCards()
    {
        Console.Clear();
        Console.Write("Deal Cards");
        Thread.Sleep(800);
        Console.Write(".");
        Thread.Sleep(800);
        Console.Write(".");
        Thread.Sleep(800);
        Console.Write(".");
        Thread.Sleep(800);
        Console.Clear();
    }

    public void PrintEndGame()
    {
        Console.WriteLine("See ya, suckers!");
        Console.WriteLine();
        Console.WriteLine(">> T H E  E N D <<");
        Console.WriteLine();
    }

    public void PrintWinnerOfRound(string name, bool shortGame)
    {
        Console.WriteLine(name + " is a winner of this round!");
        Console.WriteLine();
        if (!shortGame)
        {
            Console.WriteLine("Counting Points...");
            Thread.Sleep(5000);
            Console.WriteLine("Dealing cards...");
            Thread.Sleep(2000);
            Console.WriteLine();
        }
    }

    public void PrintWinnerPoints(Player player)
    {
        Console.WriteLine();
        Console.WriteLine(player.Name + " has now " + player.Points + " points!");
        Console.WriteLine();
    }
}