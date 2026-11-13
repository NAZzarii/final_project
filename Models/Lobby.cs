namespace GameMatch.Models;

public class Lobby
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int GameId { get; set; }
    public Game? Game { get; set; }

    public string HostId { get; set; } = string.Empty;
    public ApplicationUser? Host { get; set; }

    public int PlayersNeeded { get; set; } = 1;
    public bool HasMic { get; set; }
    public DateTime PlayTime { get; set; }

    // Видно тільки хосту та гравцям із прийнятою заявкою
    public string? ContactLink { get; set; }

    public bool IsOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<LobbyRequest> Requests { get; set; } = new();

    // Лише для відображення (потрібно завантажити Requests)
    public int FilledSlots => Requests.Count(r => r.Status == RequestStatus.Accepted);
}
