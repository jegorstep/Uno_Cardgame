using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Game = GameEngine.Game;
using Player = Domain.Database.Player;

namespace WebApp.Pages_Players
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        
        
        [BindProperty(SupportsGet = true)]
        public Guid? GameId { get; set; }
        

        private IGameRepository gameRepository = default!;

        public IndexModel(AppDbContext context)
        {
            _context = context; 
            gameRepository = new GameRepositoryEF(_context);

        }

        public IList<Player> Player { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Players != null)
            {
                Player = await _context.Players
                .Include(p => p.Game)
                .Where(p=> p.GameId.Equals(GameId))
                .ToListAsync();
            }
        }
    }
}
