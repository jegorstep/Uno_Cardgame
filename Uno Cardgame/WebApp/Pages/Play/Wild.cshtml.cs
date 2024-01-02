using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Wild : PageModel
{
    
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    
    [BindProperty(SupportsGet = true)] public string? Card { get; set; }

    [BindProperty(SupportsGet = true)] public string? Name { get; set; }
    
    
    public void OnGet()
    {
        
    }
}