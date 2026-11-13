namespace GameMatch.Models;

public enum RequestStatus { Pending, Accepted, Rejected }

public class LobbyRequest
{
    public int Id { get; set; }

    public int LobbyId { get; set; }
    public Lobby? Lobby { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
