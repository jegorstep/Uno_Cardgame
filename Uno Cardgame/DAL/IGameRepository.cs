using Domain;

namespace DAL;

public interface IGameRepository<TKey>
{
    TKey SaveGame(string name, GameState game);
    GameState LoadGame(TKey id);
}