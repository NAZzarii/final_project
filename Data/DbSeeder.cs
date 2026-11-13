using GameMatch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Data;

public static class DbSeeder
{
    public const string AdminEmail = "admin@gamematch.local";
    public const string AdminPassword = "Admin123";

    public static async Task SeedAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();
        var users = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = sp.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.EnsureCreatedAsync();

        if (!await roles.RoleExistsAsync("Admin"))
            await roles.CreateAsync(new IdentityRole("Admin"));

        if (await users.FindByEmailAsync(AdminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = AdminEmail, Email = AdminEmail, EmailConfirmed = true,
                DiscordTag = "Admin#0001", AboutMe = "Адміністратор GameMatch"
            };
            if ((await users.CreateAsync(admin, AdminPassword)).Succeeded)
                await users.AddToRoleAsync(admin, "Admin");
        }

        if (!await db.Games.AnyAsync())
        {
            db.Games.AddRange(
                new Game { Name = "CS2" }, new Game { Name = "Dota 2" },
                new Game { Name = "Valorant" }, new Game { Name = "Minecraft" },
                new Game { Name = "Fortnite" });
            await db.SaveChangesAsync();
        }

        // Іконки для стандартних ігор (також оновлює вже створену базу)
        var icons = new Dictionary<string, string>
        {
            ["CS2"] = "/img/games/cs2.svg", ["Dota 2"] = "/img/games/dota2.svg",
            ["Valorant"] = "/img/games/valorant.svg", ["Minecraft"] = "/img/games/minecraft.svg",
            ["Fortnite"] = "/img/games/fortnite.svg"
        };
        foreach (var g in await db.Games.Where(x => x.ImageUrl == null).ToListAsync())
            if (icons.TryGetValue(g.Name, out var url)) g.ImageUrl = url;
        await db.SaveChangesAsync();

        // Демо-дані, щоб головна сторінка не була порожньою
        if (!await db.Lobbies.AnyAsync())
        {
            var kostya = await CreateDemoUser(users, "kostya@demo.local", "Kostya#1234", "Граю в CS2 з 2016, шукаю адекватних тіммейтів");
            var nikita = await CreateDemoUser(users, "nikita@demo.local", "Nikita#5678", "Дота і майнкрафт по вечорах");
            var games = await db.Games.ToDictionaryAsync(g => g.Name);

            if (kostya != null && nikita != null)
            {
                db.Lobbies.AddRange(
                    new Lobby { Title = "Faceit стак на вечір", Description = "Шукаємо 2 гравців, від 5 рівня. Без токсиків, голос обов'язково.", GameId = games["CS2"].Id, HostId = kostya.Id, PlayersNeeded = 2, HasMic = true, PlayTime = DateTime.Now.AddHours(3), ContactLink = "https://discord.gg/demo-cs2" },
                    new Lobby { Title = "Рейтинг Dota 2 (Divine+)", Description = "Потрібен керрі та хард-саппорт.", GameId = games["Dota 2"].Id, HostId = nikita.Id, PlayersNeeded = 2, HasMic = true, PlayTime = DateTime.Now.AddHours(5), ContactLink = "https://discord.gg/demo-dota" },
                    new Lobby { Title = "Виживання на сервері друзів", Description = "Будуємо базу, мікрофон не обов'язковий.", GameId = games["Minecraft"].Id, HostId = nikita.Id, PlayersNeeded = 3, HasMic = false, PlayTime = DateTime.Now.AddDays(1), ContactLink = "https://discord.gg/demo-mc" });
                await db.SaveChangesAsync();
            }
        }
    }

    private static async Task<ApplicationUser?> CreateDemoUser(UserManager<ApplicationUser> users, string email, string discord, string about)
    {
        var existing = await users.FindByEmailAsync(email);
        if (existing != null) return existing;
        var u = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, DiscordTag = discord, AboutMe = about };
        return (await users.CreateAsync(u, "Demo123")).Succeeded ? u : null;
    }
}
