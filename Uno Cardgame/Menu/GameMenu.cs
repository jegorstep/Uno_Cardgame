using DAL;
using GameEngine;
using Microsoft.EntityFrameworkCore;

namespace Menu;

public class GameMenu
{
    private IGameRepository _gameRepository = default!;
    private GameUI _gameUi = new GameUI();
    
    

    private string _title;
    private int _players = 2;
    private string _gameType = "Official";

    public GameMenu(string title)
    {
        _title = title;
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
        string exitOrBack = "";
        
        string[] options = {"Start Game", "Load Game", "Exit" };
        while (exitOrBack != "x")
        {
            int answer = _gameUi.UniversalMenu(_title, options);
            
            if (answer == 0)
            {
                exitOrBack = NewGameFormat();
            }
            else if (answer == 1)
            {
                exitOrBack = LoadGameFormat();
            }
            else if (answer == 2)
            {
                exitOrBack = "x";
            }
        }

        return exitOrBack;
    }

    private string NewGameFormat()
    {
        var exitOrBack = "";
        

        while (exitOrBack != "x" && exitOrBack != "b")
        {
            string[] options = { "Play", "Number of players (choose from 2 to 10): " + _players,
                "Official or custom: "  + _gameType, "Back", "Exit"};
            
            int answer = _gameUi.UniversalMenu("New Game", options);
            
            if (answer == 0)
            {
                Game game = new Game(_players, _gameType);
                game.Run();
                exitOrBack = "b";
            }
            else if (answer == 1)
            {
                _players = _gameUi.DemandNumber("choose from 2 to 10: ", 2, 10);
            }
            else if (answer == 2)
            {
                string[] smallOptions = { "Official", "Custom"};
                _gameType = smallOptions[_gameUi.UniversalMenu("GAME TYPE", smallOptions)];
            } else if (answer == 3)
            {
                exitOrBack = "b";
            }
            else
            {
                exitOrBack = "x";
            }
        }

        return exitOrBack;



    } 

    private string LoadGameFormat()
    {
        string exitOrBack = "";
        while (exitOrBack != "b" && exitOrBack != "x")
        {
            // if web application, using database
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=" + Path.Combine(Path.GetTempPath(), "savedGamesDb"))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;
            using var db = new AppDbContext(contextOptions);
            db.Database.Migrate();
            _gameRepository = new GameRepositoryEF(db);
            // end of database code
            
            
            
            // if file save system
            // _gameRepository = new GameRepositoryFileSystem();
            // end of file save system
            
            
            
            var savedGames = _gameRepository.GetSaveGames();
            List<string> abc = new List<string>();
            foreach (var saveGame in savedGames)
            {
                
                var tempString = saveGame.ToString();
                abc.Add(tempString.Trim());
            }
            abc.Add("Back");
            abc.Add("Exit");
            
            string[] abcArray = abc.ToArray();
            
            int answer = _gameUi.UniversalMenu("Load Game", abcArray);

            if (answer <= savedGames.Count - 1)
            {
                Game game = new Game(_gameRepository.LoadGame(savedGames[answer].Item1));
                game.Run();
            }

            else if (answer == savedGames.Count)
            {
                exitOrBack = "b";
            }
            else if (answer == savedGames.Count() + 1)
            {
                exitOrBack = "x";
            }
        }

        return exitOrBack;
    }
    
}