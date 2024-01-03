using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Log : PageModel
{
    
    public GameState GameState { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    
    [BindProperty(SupportsGet = true)] public Guid Name { get; set; }
    
    [BindProperty(SupportsGet = true)] public string? Winner { get; set; }
    
    private readonly AppDbContext _context;

    private IGameRepository _gameRepository;

    public string[] LoggedInfo = default!;
    
    public Log(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }
    
    
    public IActionResult OnGet()
    {
        GameState = _gameRepository.LoadGame(GameId);

        Game game = new Game(GameState);
        
        if (GameState.Log != null)
        {
            LoggedInfo = GameState.Log.Split("\n");
        }
        
        if (Winner != null)
        {
            Player winnerObject = default!;
            foreach (var player in GameState.Players)
            {
                if (player.Name.Equals(Winner))
                {
                    winnerObject = player;
                }
            }
            if (game.CountPoints(winnerObject))
            {
                game.SaveGame();
                return Redirect("/Play/GameOver?gameId=" + GameId + "&name=" + Name);
            }
            game.SaveGame();
            return Redirect("/Play/Index?gameId=" + GameId + "&name=" + Name);
        }
        return Page();
    }
}