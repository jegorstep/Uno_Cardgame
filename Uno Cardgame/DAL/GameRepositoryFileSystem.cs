using System.Text.Json;
using DAL;
using Domain;


public class GameRepositoryFileSystem : IGameRepository<string>
{
    // private static int? id;
    private string _filePrefix = Path.GetTempPath();

    public string SaveGame(string name, GameState game)
    {
        _filePrefix = Path.Combine(_filePrefix, "savedGames");
        if (!Directory.Exists(_filePrefix))
        {
            Directory.CreateDirectory(_filePrefix);
        }
        Console.WriteLine(_filePrefix);
        var fileName = name + ".json";
        var filePath = Path.Combine(_filePrefix, fileName);
        Console.WriteLine(filePath);
        File.WriteAllText(filePath, JsonSerializer.Serialize(game));
        return fileName; 
    }

    public GameState LoadGame(string path)
    {
        return JsonSerializer.Deserialize<GameState>(
            System.IO.File.ReadAllText(path)
        )!;
    }
}