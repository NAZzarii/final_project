using System.ComponentModel.DataAnnotations;

namespace GameMatch.Models;

public class CreateLobbyVm
{
    [Required(ErrorMessage = "Вкажіть назву"), StringLength(80)]
    [Display(Name = "Назва лобі")]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Оберіть гру")]
    [Display(Name = "Гра")]
    public int GameId { get; set; }

    [StringLength(500)]
    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Range(1, 20, ErrorMessage = "Від 1 до 20")]
    [Display(Name = "Скільки гравців потрібно")]
    public int PlayersNeeded { get; set; } = 1;

    [Display(Name = "Є мікрофон")]
    public bool HasMic { get; set; } = true;

    [Display(Name = "Час гри")]
    public DateTime PlayTime { get; set; }

    [StringLength(200)]
    [Display(Name = "Посилання на Discord / канал")]
    public string? ContactLink { get; set; }
}

public class LobbyDetailsVm
{
    public Lobby Lobby { get; set; } = null!;
    public List<LobbyRequest> Accepted { get; set; } = new();
    public List<LobbyRequest> Pending { get; set; } = new();
    public LobbyRequest? MyRequest { get; set; }
    public bool IsHost { get; set; }
    public bool CanSeeContacts { get; set; }
}

public class ProfileEditVm
{
    [StringLength(40)]
    [Display(Name = "Discord")]
    public string? DiscordTag { get; set; }

    [StringLength(200)]
    [Display(Name = "Посилання на Steam")]
    public string? SteamLink { get; set; }

    [StringLength(300)]
    [Display(Name = "Про себе")]
    public string? AboutMe { get; set; }
}
