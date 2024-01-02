using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Pages_Players
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        
        
        [BindProperty(SupportsGet = true)]
        public Guid? GameId { get; set; }

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;

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
