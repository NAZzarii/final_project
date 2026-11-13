using GameMatch.Data;
using GameMatch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Controllers;

[Authorize]
public class ProfileController(ApplicationDbContext db, UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> Index()
    {
        var user = (await users.GetUserAsync(User))!;
        ViewBag.LobbyCount = await db.Lobbies.CountAsync(l => l.HostId == user.Id);
        ViewBag.RequestCount = await db.LobbyRequests.CountAsync(r => r.UserId == user.Id);
        return View(user);
    }

    public async Task<IActionResult> Edit()
    {
        var u = (await users.GetUserAsync(User))!;
        return View(new ProfileEditVm { DiscordTag = u.DiscordTag, SteamLink = u.SteamLink, AboutMe = u.AboutMe });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileEditVm vm)
    {
        if (!string.IsNullOrWhiteSpace(vm.SteamLink) && !UiHelpers.IsHttp(vm.SteamLink))
            ModelState.AddModelError(nameof(vm.SteamLink), "Вкажіть повне посилання (https://...)");
        if (!ModelState.IsValid) return View(vm);

        var u = (await users.GetUserAsync(User))!;
        u.DiscordTag = vm.DiscordTag?.Trim();
        u.SteamLink = vm.SteamLink?.Trim();
        u.AboutMe = vm.AboutMe?.Trim();
        await users.UpdateAsync(u);
        TempData["Success"] = "Профіль збережено.";
        return RedirectToAction(nameof(Index));
    }
}
