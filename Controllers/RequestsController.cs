using GameMatch.Data;
using GameMatch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Controllers;

[Authorize]
public class RequestsController(ApplicationDbContext db, UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> My()
    {
        var userId = users.GetUserId(User);
        var list = await db.LobbyRequests
            .Include(r => r.Lobby).ThenInclude(l => l!.Game)
            .Include(r => r.Lobby).ThenInclude(l => l!.Host)
            .Where(r => r.UserId == userId).OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int lobbyId)
    {
        var userId = users.GetUserId(User)!;
        var lobby = await db.Lobbies.Include(l => l.Requests).FirstOrDefaultAsync(l => l.Id == lobbyId);
        if (lobby == null) return NotFound();

        if (lobby.HostId == userId)
            TempData["Error"] = "Не можна подати заявку у власне лобі.";
        else if (!lobby.IsOpen)
            TempData["Error"] = "Лобі закрите.";
        else if (lobby.Requests.Any(r => r.UserId == userId))
            TempData["Error"] = "Ви вже подавали заявку в це лобі.";
        else
        {
            db.LobbyRequests.Add(new LobbyRequest { LobbyId = lobbyId, UserId = userId });
            await db.SaveChangesAsync();
            TempData["Success"] = "Заявку подано! Чекайте відповіді хоста.";
        }
        return RedirectToAction("Details", "Lobbies", new { id = lobbyId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int id)
    {
        var req = await db.LobbyRequests.Include(r => r.Lobby).ThenInclude(l => l!.Requests)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (req == null) return NotFound();

        var lobby = req.Lobby!;
        if (lobby.HostId != users.GetUserId(User)) return Forbid(); // тільки хост

        if (req.Status != RequestStatus.Pending)
            TempData["Error"] = "Цю заявку вже опрацьовано.";
        else if (lobby.FilledSlots >= lobby.PlayersNeeded)
            TempData["Error"] = "Усі місця вже зайняті.";
        else
        {
            req.Status = RequestStatus.Accepted;
            if (lobby.FilledSlots >= lobby.PlayersNeeded) lobby.IsOpen = false; // набрали команду
            await db.SaveChangesAsync();
            TempData["Success"] = "Гравця прийнято.";
        }
        return RedirectToAction("Details", "Lobbies", new { id = lobby.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var req = await db.LobbyRequests.Include(r => r.Lobby).FirstOrDefaultAsync(r => r.Id == id);
        if (req == null) return NotFound();
        if (req.Lobby!.HostId != users.GetUserId(User)) return Forbid(); // тільки хост

        if (req.Status == RequestStatus.Pending)
        {
            req.Status = RequestStatus.Rejected;
            await db.SaveChangesAsync();
            TempData["Success"] = "Заявку відхилено.";
        }
        return RedirectToAction("Details", "Lobbies", new { id = req.LobbyId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var req = await db.LobbyRequests.FindAsync(id);
        if (req == null) return NotFound();
        if (req.UserId != users.GetUserId(User)) return Forbid();

        if (req.Status == RequestStatus.Pending)
        {
            db.LobbyRequests.Remove(req);
            await db.SaveChangesAsync();
            TempData["Success"] = "Заявку скасовано.";
        }
        return RedirectToAction(nameof(My));
    }
}
