using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;


namespace MovieAppsProject.Pages;

[Authorize]                                   // signed-in users
public class SecurePrivacyModel : PageModel
{
    private readonly ILogger<SecurePrivacyModel> _logger;

    public SecurePrivacyModel(ILogger<SecurePrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

