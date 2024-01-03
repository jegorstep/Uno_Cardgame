using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class StartGame : PageModel
{
    [BindProperty]
    public int TotalPlayers { get; set; }

    [BindProperty]
    public List<string>? AllPlayerNames { get; set; }

    public string? ErrorMessage;
    
    public void OnGet()
    {
        // This is called when the page is initially requested
    }

    public IActionResult OnPost()
    {
        //TODO make wrong data return back to page in the same fields (with same totalPlayers and humanPlayers counts)
        
        int realPlayersCount = AllPlayerNames!.Count;
        for (int i = realPlayersCount; i < TotalPlayers; i++)
        {
            AllPlayerNames.Add($"AI Player {i + 1}");
        }

        if (ValidationFunc())
        {
            return Page();
        }

        Game game = new Game(AllPlayerNames.Count, "official");
        game.SetUpGame(AllPlayerNames, realPlayersCount);
        game.SaveGame();
        
        return Redirect("/Games/Index");
    }

    private bool ValidationFunc()
    {
        bool minLength = false;
        bool maxLength = false;
        bool copyNames = false;
        ErrorMessage = "";
        foreach (string player in AllPlayerNames!)
        {
            string playerString = player.Trim(); 
            if (playerString == null)
            {
                if (!ErrorMessage.Contains("Name length must be longer than 2 letters"))
                {
                    ErrorMessage += "Name length must be longer than 2 letters!\n";
                }

                minLength = true;
            }
            else if (playerString.Length < 2 && !minLength)
            {
                ErrorMessage += "Name length must be longer than 2 letters!\n";
                minLength = true;
            }
            else if (playerString.Length > 20 && !maxLength)
            {
                ErrorMessage += "Name length must be not longer than 20 letters!\n";
                maxLength = true;
            }

            else if (AllPlayerNames.Count != AllPlayerNames.Distinct().Count() && !copyNames)
            {
                ErrorMessage += "No duplicates allowed!\n";
                copyNames = true;
            }
        }

        return minLength || maxLength || copyNames;
    }

}