using Domain;

namespace GameEngine;

public class GameUI
{
    
    static int selectedOption = 0;

    private const string MenuSeparator = "=======================";

    public void UniversalMenu(string[] options)
    {
        Console.CursorVisible = false;
        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            PrintMenu(options);

            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedOption > 0)
                        selectedOption--;
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedOption < options.Length - 1)
                        selectedOption++;
                    break;
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.Clear();
        Console.WriteLine($"You selected: {options[selectedOption]}");
    }

    static void PrintMenu(string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (i == selectedOption)
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

    public void PrintWinnerOfRound(string name)
    {
        Console.WriteLine(name + " is a winner of this round!");
        Console.WriteLine();
        Console.WriteLine("Counting Points...");
        Thread.Sleep(5000);
        Console.WriteLine("Dealing cards...");
        Thread.Sleep(2000);
        Console.WriteLine();
    }

    public void PrintWinnerPoints(Player player)
    {
        Console.WriteLine();
        Console.WriteLine(player.Name + " has now " + player.Points + " points!");
        Console.WriteLine();
    }

    public void PrintPlayerSaveOrExit()
    {
        Console.WriteLine();
        Console.WriteLine("s) Save Game");
        Console.WriteLine("x) Exit Game");
    }

    public void PrintColorChooseWild()
    {
        Console.WriteLine("y) Yellow");
        Console.WriteLine("g) Green");
        Console.WriteLine("b) Blue");
        Console.WriteLine("r) Red");
        Console.Write("Choose color: ");
    }
}