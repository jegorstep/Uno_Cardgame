using Domain;

namespace DAL;

public interface IGameRepository
{
    void Save(Guid id, GameState state);
    List<(Guid id, DateTime dt)> GetSaveGames();

    GameState LoadGame(Guid id);
}