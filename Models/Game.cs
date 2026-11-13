namespace GameMatch.Models;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<Lobby> Lobbies { get; set; } = new();
}
