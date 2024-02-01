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
    
    [BindProperty] public int FirstPlayer { get; set;}
    [BindProperty] public int CardsAmount { get; set; }
    [BindProperty] public bool CustomRules { get; set; }
    [BindProperty] public bool CustomRule1 { get; set; }
    [BindProperty] public bool CustomRule2 { get; set; }


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

        string gameType = "official";
        if (CustomRules)
        {
            gameType = "custom";
        }

        Game game = new Game(AllPlayerNames.Count, "Official");
        game.SetUpGame(AllPlayerNames, realPlayersCount, FirstPlayer, gameType, CustomRule1, CustomRule2, CardsAmount);
        game.SaveGame();
        
        return Redirect("/Games/Index");
    }

    private bool ValidationFunc()
    {
        bool minLength = false;
        bool maxLength = false;
        bool copyNames = false;
        bool cardAmountEmpty = false;
        
        ErrorMessage = "";
        if (CustomRules && CardsAmount == 0)
        {
            ErrorMessage += "Choose amount of cards you want to play with!\n";
            cardAmountEmpty = true;
        }
        foreach (string player in AllPlayerNames!)
        {
            string playerString = player; 
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

        return minLength || maxLength || copyNames || cardAmountEmpty;
    }

}