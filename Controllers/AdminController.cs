using GameMatch.Data;
using GameMatch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Games.OrderBy(g => g.Name).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string? imageUrl)
    {
        name = (name ?? "").Trim();
        if (name.Length == 0 || name.Length > 50)
            TempData["Error"] = "Вкажіть назву гри (до 50 символів).";
        else if (await db.Games.AnyAsync(g => g.Name == name))
            TempData["Error"] = "Така гра вже є.";
        else if (!string.IsNullOrWhiteSpace(imageUrl) && !UiHelpers.IsHttp(imageUrl))
            TempData["Error"] = "Посилання на логотип має починатися з http(s)://";
        else
        {
            db.Games.Add(new Game { Name = name, ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim() });
            await db.SaveChangesAsync();
            TempData["Success"] = $"Гру «{name}» додано.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var game = await db.Games.FindAsync(id);
        if (game == null) return NotFound();

        if (await db.Lobbies.AnyAsync(l => l.GameId == id))
            TempData["Error"] = "Не можна видалити гру, для якої є лобі.";
        else
        {
            db.Games.Remove(game);
            await db.SaveChangesAsync();
            TempData["Success"] = "Гру видалено.";
        }
        return RedirectToAction(nameof(Index));
    }
}
