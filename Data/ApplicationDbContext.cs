using GameMatch.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameMatch.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Lobby> Lobbies => Set<Lobby>();
    public DbSet<LobbyRequest> LobbyRequests => Set<LobbyRequest>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Game>().HasMany(g => g.Lobbies).WithOne(l => l.Game)
            .HasForeignKey(l => l.GameId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Lobby>().HasOne(l => l.Host).WithMany()
            .HasForeignKey(l => l.HostId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<LobbyRequest>().HasOne(r => r.Lobby).WithMany(l => l.Requests)
            .HasForeignKey(r => r.LobbyId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<LobbyRequest>().HasOne(r => r.User).WithMany()
            .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);

        // одна заявка від одного користувача в одне лобі
        b.Entity<LobbyRequest>().HasIndex(r => new { r.LobbyId, r.UserId }).IsUnique();
    }
}
