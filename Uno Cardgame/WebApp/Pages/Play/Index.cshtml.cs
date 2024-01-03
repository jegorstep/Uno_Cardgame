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
    [BindProperty(SupportsGet = true)] public string? TakeCard { get; set;}
    
    public Card? CurrentCard;
    
    public Player? Player;
    
    
    public IActionResult OnGet()
    {
        GameState = _gameRepository.LoadGame(GameId);

        GameEngine = new Game(GameState);

        Player? winner = GameEngine.CheckHands();
        
        foreach (Player player in GameState.Players)
        {
            if (player.Name == Name)
            {
                Player = player;
            }
        }

        if (!GameState.GetActivePlayer().IsHuman)
        {
            GameEngine.PlayerAction();
            GameEngine.SaveGame();
        }

        if (winner == null)
        {
            if (TakeCard == "true")
            {
                CurrentCard = GameEngine.NoPossibleMoves(Player!);
                if (CurrentCard != null && CurrentCard.CardColor == Domain.Card.Color.Wild)
                {
                    Player!.Hand.Add(CurrentCard);
                    GameEngine.SaveGame();
                    return Redirect("/Play/Wild?gameId=" + GameId + "&name=" + Name + "&card=" + CurrentCard);
                }
                else if (CurrentCard != null && CurrentCard.CardValue == Domain.Card.Value.Seven && GameState.SwappingCards)
                {
                    Player!.Hand.Add(CurrentCard);
                    GameEngine.SaveGame();
                    return Redirect("/Play/Swap?gameId=" + GameId + "&name=" + Name + "&card=" + CurrentCard);
                }
                GameEngine.ActionManager(Player!, CurrentCard);
                GameEngine.SaveGame();
                return Redirect("/Play?gameId=" + GameId + "&name=" + Name);
                
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
                if (CurrentCard!.CardColor == Domain.Card.Color.Wild)
                {
                    GameEngine.WildForWeb(CurrentCard, Color!);
                }
                else
                {
                    GameEngine.ActionManager(Player, CurrentCard);
                }

                GameEngine.SaveGame();

                return Redirect("/Play?gameId=" + GameId + "&name=" + Name);
            }
        }
        else
        {
            if (GameEngine.CountPoints(winner)) 
            {
                GameEngine.SaveGame();
                return Redirect("/Play/Gameover?gameId=" + GameId + "&name=" + Name + "&winner=" + winner.Name); 
            }
            GameEngine.SaveGame();
            return Redirect("/Play/Log?gameId=" + GameId + "&name=" + Name + "&winner=" + winner.Name); 
        }
        return Page();
    }
}