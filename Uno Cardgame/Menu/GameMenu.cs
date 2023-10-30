using GameEngine;
namespace Menu;

public class GameMenu
{
    private string Title;
    public int Players = 2;
    public int AI = 0;
    public string GameType = "Official";
    
    
    private const string MenuSeparator = "=======================";


    public GameMenu(string title)
    {
        Title = title;
    }

    public void Run()
    {
        var answer = "";
        while (!answer.ToLower().Equals("x"))
        {
            answer = StartFormat();
            Console.Clear();
        }

    }

    private string StartFormat()
    {
        var answer = "";
        while (!answer.ToLower().Equals("x"))
        {
            do
            {
                Console.WriteLine(Title);
                Console.WriteLine(MenuSeparator);
                Console.WriteLine("s) Start Game");
                Console.WriteLine("l) Load Game");
                Console.WriteLine("x) eXit");
                Console.WriteLine(MenuSeparator);
                Console.Write("Your choice: ");

                answer = Console.ReadLine()?.Trim();
                Console.WriteLine(answer);

                if (!answer.ToLower().Equals("s") && !answer.ToLower().Equals("l") && !answer.ToLower().Equals("x"))
                {
                    Console.Clear();
                    Console.WriteLine("Wrong input!");
                }
            } while (!answer.ToLower().Equals("s") && !answer.ToLower().Equals("l") && !answer.ToLower().Equals("x"));

            if (answer.ToLower().Equals("s"))
            {
                Console.Clear();
                answer = NewGameFormat();
            }
            else if (answer.ToLower().Equals("l"))
            {
                Console.Clear();
                answer = LoadGameFormat();
            }
        }
        Console.Clear();
        return answer;
    }

    private string NewGameFormat()
    {
        var answer = "";
        while (!answer.ToLower().Equals("x") && !answer.ToLower().Equals("b"))
        {
            do
            {
                Console.WriteLine("New Game");
                Console.WriteLine(MenuSeparator);
                Console.WriteLine("p) Play");
                Console.WriteLine("n) Number of players (choose from 2 to 10): " + Players);
                Console.WriteLine("o) Official or custom: " + GameType);
                Console.WriteLine("b) Back");
                Console.WriteLine("x) eXit");
                Console.WriteLine(MenuSeparator);
                Console.Write("Your choice: ");

                answer = Console.ReadLine()?.Trim();

                if (!answer.ToLower().Equals("p") && !answer.ToLower().Equals("b") && !answer.ToLower().Equals("x") &&
                    !answer.ToLower().Equals("n") && !answer.ToLower().Equals("o"))
                {
                    Console.Clear();
                    Console.WriteLine("Wrong input!");
                }

                if (answer.ToLower().Equals("p"))
                {
                    Game game = new Game(Players);
                    game.Run();
                    answer = "b";
                }
                
                else if (answer.ToLower().Equals("o"))
                {
                    Console.Clear();
                    Console.WriteLine("1)Official");
                    Console.WriteLine("2)Custom");
                    Console.WriteLine("Choose game type (Right now this feature doesn't work and game will always be official) : ");
                    
                    var input = Console.ReadLine()?.Trim();
                    
                    if (input.ToLower().Equals("2") || input.ToLower().Equals("c"))
                    {
                        Console.Clear();
                        GameType = "Custom";
                    }
                    else if (input.ToLower().Equals("1") || input.ToLower().Equals("o") )
                    {
                        Console.Clear();
                        GameType = "Official";
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Wrong input! Must choose official or custom");
                    }
                } 
                else if (answer.ToLower().Equals("n"))
                {
                    Console.Clear();
                    Console.WriteLine("Choose number of players: ");
                    var number = Console.ReadLine()?.Trim();
                    var integer = 0;
                    if (int.TryParse(number, out integer))
                    {
                        if (integer is >= 2 and <= 10)
                        {
                            Players = integer;
                            Console.Clear();
                            Console.WriteLine("Player amount has been successfully changed!");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Number of players must be from 2 to 10!");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Input is incorrect, player amount is not changed!");
                        Console.WriteLine();
                    }
                }

            } while (!answer.ToLower().Equals("b") && !answer.ToLower().Equals("x"));
        }
        Console.Clear();
        return answer;
    } 

    private string LoadGameFormat()
    {
        var answer = "";
        do
        {
            Console.WriteLine("Load Game");
            Console.WriteLine(MenuSeparator);
            var directory = Path.Combine(Path.GetTempPath(), "savedGames");
            string[] files = Directory.GetFiles(directory);
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine(files[i]);
            }
            var txtPath = Path.Combine(Path.GetTempPath(), "saved_games.txt");
            if (!File.Exists(txtPath))
            {
                File.CreateText(txtPath);
            }

            using (StreamReader reader = new StreamReader(txtPath))
            {
                string line;
                var x = 1;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(x + ") " + line);
                    x++;
                }
            }
            Console.WriteLine();
            Console.WriteLine("b) Back");
            Console.WriteLine("x) eXit");
            Console.WriteLine(MenuSeparator);
            Console.Write("Your Choice: ");
            answer = Console.ReadLine()?.Trim();
            var saveNumber = 0;
            if (int.TryParse(answer, out saveNumber))
            {
                if (saveNumber > 0 && files.Length >= saveNumber)
                {
                    GameRepositoryFileSystem gameRepositoryFileSystem = new GameRepositoryFileSystem();
                    Game game = new Game(gameRepositoryFileSystem.LoadGame(files[saveNumber - 1]));
                    game.Run();
                }
            }
            if (!answer.ToLower().Equals("x") && !answer.ToLower().Equals("b"))
            {
                Console.Clear();
                Console.WriteLine("Wrong input!");
            }
        } while (!answer.ToLower().Equals("x") && !answer.ToLower().Equals("b"));
        
        Console.Clear();
        return answer;
    }
    
    
}