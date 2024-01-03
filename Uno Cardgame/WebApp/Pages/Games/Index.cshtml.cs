
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Database;

namespace WebApp.Pages_Games
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Games != null)
            {
                Game = await _context.Games.Include(g => g.Players).OrderByDescending(g => g.UpdatedAtDt)
                    .ToListAsync();
            }
        }
    }
}