using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly AppDbContext _context;

    private IGameRepository _gameRepository;
    
    

    public Index(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }
    
    public Game GameEngine { get; set; } = default!;

    public GameState GameState { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)] public string? Card { get; set; }
    [BindProperty(SupportsGet = true)] public string? Color { get; set; }
    [BindProperty(SupportsGet = true)] public string? Name { get; set; }
    
    public Card CurrentCard = default!;
    
    public Player? Player;
    
    
    public IActionResult OnGet()
    {
        GameState = _gameRepository.LoadGame(GameId);

        GameEngine = new Game(GameState);
        
        foreach (Player player in GameState.Players)
        {
            if (player.Name == Name)
            {
                Player = player;
            }
        }

        if (Card != null)
        {
            foreach (var card in Player!.Hand)
            {
                if (card.ToString().Equals(Card))
                {
                    CurrentCard = card;
                }
            }
            if (CurrentCard.CardColor == Domain.Card.Color.Wild)
            {
                GameEngine.WildForWeb(CurrentCard, Color!);
            }
            else
            {
                GameEngine.ActionManager(Player, CurrentCard);
            }

            GameEngine.SaveGame();

            return RedirectToPage("/Play?gameId=" + GameId + "&name=" + Name);
        }
        return Page();
    }
}