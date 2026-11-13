using GameMatch.Data;
using GameMatch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Controllers;

public class HomeController(ApplicationDbContext db, UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> Index(int? gameId)
    {
        var query = db.Lobbies.Include(l => l.Game).Include(l => l.Host).Include(l => l.Requests)
            .Where(l => l.IsOpen);
        if (gameId.HasValue) query = query.Where(l => l.GameId == gameId.Value);

        ViewBag.Games = await db.Games.OrderBy(g => g.Name).ToListAsync();
        ViewBag.SelectedGameId = gameId;

        var userId = users.GetUserId(User);
        ViewBag.AcceptedCount = userId == null ? 0 :
            await db.LobbyRequests.CountAsync(r => r.UserId == userId && r.Status == RequestStatus.Accepted);

        return View(await query.OrderByDescending(l => l.CreatedAt).ToListAsync());
    }

    public IActionResult Error() => View();
}
