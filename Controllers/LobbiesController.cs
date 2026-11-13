using GameMatch.Data;
using GameMatch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Controllers;

[Authorize]
public class LobbiesController(ApplicationDbContext db, UserManager<ApplicationUser> users) : Controller
{
    private async Task LoadGames() =>
        ViewBag.Games = new SelectList(await db.Games.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");

    public async Task<IActionResult> Create()
    {
        await LoadGames();
        return View(new CreateLobbyVm { PlayTime = DateTime.Now.AddHours(1) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLobbyVm vm)
    {
        if (!await db.Games.AnyAsync(g => g.Id == vm.GameId))
            ModelState.AddModelError(nameof(vm.GameId), "Оберіть гру");
        if (vm.PlayTime < DateTime.Now.AddMinutes(-5))
            ModelState.AddModelError(nameof(vm.PlayTime), "Час гри вже минув");

        if (!ModelState.IsValid)
        {
            await LoadGames();
            return View(vm);
        }

        var lobby = new Lobby
        {
            Title = vm.Title.Trim(),
            Description = vm.Description?.Trim(),
            GameId = vm.GameId,
            HostId = users.GetUserId(User)!,
            PlayersNeeded = vm.PlayersNeeded,
            HasMic = vm.HasMic,
            PlayTime = vm.PlayTime,
            ContactLink = vm.ContactLink?.Trim()
        };
        db.Lobbies.Add(lobby);
        await db.SaveChangesAsync();
        TempData["Success"] = "Лобі створено!";
        return RedirectToAction(nameof(Details), new { id = lobby.Id });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var lobby = await db.Lobbies.Include(l => l.Game).Include(l => l.Host)
            .Include(l => l.Requests).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (lobby == null) return NotFound();

        var userId = users.GetUserId(User);
        var isHost = userId != null && lobby.HostId == userId;
        var mine = userId == null ? null : lobby.Requests.FirstOrDefault(r => r.UserId == userId);

        return View(new LobbyDetailsVm
        {
            Lobby = lobby,
            Accepted = lobby.Requests.Where(r => r.Status == RequestStatus.Accepted).ToList(),
            Pending = lobby.Requests.Where(r => r.Status == RequestStatus.Pending).ToList(),
            MyRequest = mine,
            IsHost = isHost,
            // перевірка на сервері: контакти лише хосту і прийнятим
            CanSeeContacts = isHost || mine?.Status == RequestStatus.Accepted
        });
    }

    public async Task<IActionResult> My()
    {
        var userId = users.GetUserId(User);
        var list = await db.Lobbies.Include(l => l.Game).Include(l => l.Requests)
            .Where(l => l.HostId == userId).OrderByDescending(l => l.CreatedAt).ToListAsync();
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleOpen(int id)
    {
        var lobby = await db.Lobbies.Include(l => l.Requests).FirstOrDefaultAsync(l => l.Id == id);
        if (lobby == null) return NotFound();
        if (lobby.HostId != users.GetUserId(User)) return Forbid();

        if (!lobby.IsOpen && lobby.FilledSlots >= lobby.PlayersNeeded)
            TempData["Error"] = "Усі місця вже зайняті, відкрити лобі не можна.";
        else
        {
            lobby.IsOpen = !lobby.IsOpen;
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lobby = await db.Lobbies.FindAsync(id);
        if (lobby == null) return NotFound();
        if (lobby.HostId != users.GetUserId(User)) return Forbid();

        db.Lobbies.Remove(lobby);
        await db.SaveChangesAsync();
        TempData["Success"] = "Лобі видалено.";
        return RedirectToAction(nameof(My));
    }
}
