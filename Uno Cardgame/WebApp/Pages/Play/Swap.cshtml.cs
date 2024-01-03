using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;



public class Swap : PageModel
{
    private readonly AppDbContext _context;
    private GameRepositoryEF _gameRepository;

    public Swap(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }
    
    public Game GameEngine { get; set; } = default!;

    public GameState GameState { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)] public string? Card { get; set; }
    [BindProperty(SupportsGet = true)] public string? Name { get; set; }
    
    [BindProperty(SupportsGet = true)] public string? PlayerToSwapCards { get; set; }
    
    public Card? CurrentCard;
    
    public Player? Player;

    public List<string> PlayersNames = default!;


    public IActionResult OnGet()
    {
        GameState = _gameRepository.LoadGame(GameId);

        GameEngine = new Game(GameState);

        Player? PlayerToSwap = null;

        foreach (Player player in GameState.Players)
        {
            if (player.Name == Name)
            {
                Player = player;
            }
            else if (PlayerToSwapCards != null && player.Name == PlayerToSwapCards)
            {
                PlayerToSwap = player;
            }
        }
        foreach (var card in Player!.Hand)
        {
            if (card.ToString().Equals(Card))
            {
                CurrentCard = card;
            }
        }

        if (PlayerToSwap != null)
        {
            Player.Hand.Remove(CurrentCard);
            GameState.LastCardOnDiscardPile = CurrentCard;
            (Player!.Hand, PlayerToSwap.Hand) = (PlayerToSwap.Hand, Player.Hand);
            GameState.Log += Player.Name + " plays card " + CurrentCard + "\n";
            GameState.Log += Player.Name + " swapped cards with " + PlayerToSwap.Name + "\n";
            GameEngine.SkipPlayer();
            GameEngine.SaveGame();
            return Redirect("/Play/Index?gameId=" + GameId + "&name=" + Name);
        }


        if (CurrentCard!.CardValue == Domain.Card.Value.Seven && GameState.SwappingCards)
        {
            List<string> playersNames = new List<string>();
            List<Player> players = new List<Player>();

            foreach (var igrok in GameState.Players)
            {
                if (!Player.Equals(igrok))
                {
                    playersNames.Add(igrok.Name);
                    players.Add(igrok);
                }
            }
            PlayersNames = playersNames;
        }
        return Page();
    }
}