using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Log : PageModel
{
    
    public GameState GameState { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    
    private readonly AppDbContext _context;

    private IGameRepository _gameRepository;

    public string[] LoggedInfo = default!;
    
    public Log(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }
    
    
    public void OnGet()
    {
        GameState = _gameRepository.LoadGame(GameId);
        LoggedInfo = GameState.Log.Split("\n");
        
    }
}