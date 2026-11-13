using Microsoft.AspNetCore.Identity;

namespace GameMatch.Models;

public class ApplicationUser : IdentityUser
{
    public string? DiscordTag { get; set; }
    public string? SteamLink { get; set; }
    public string? AboutMe { get; set; }

    // Не зберігається в БД: нік = частина email до "@"
    public string DisplayName => (UserName ?? "user").Split('@')[0];
}
