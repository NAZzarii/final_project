using Microsoft.AspNetCore.Html;

namespace GameMatch.Models;

public static class UiHelpers
{
    public static bool IsHttp(string? s) =>
        Uri.TryCreate(s, UriKind.Absolute, out var u) &&
        (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);

    // "CS2" -> "cs2", "Dota 2" -> "dota2" (для кольору гри в CSS)
    public static string Slug(string? name) =>
        new string((name ?? "").ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());

    public static string When(DateTime t)
    {
        var days = (t.Date - DateTime.Now.Date).Days;
        var time = t.ToString("HH:mm");
        return days == 0 ? $"Сьогодні {time}" : days == 1 ? $"Завтра {time}" : $"{t:dd.MM} {time}";
    }
}

public static class Icons
{
    private static readonly Dictionary<string, string> Paths = new()
    {
        ["home"] = "<path d='M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z'/><polyline points='9 22 9 12 15 12 15 22'/>",
        ["list"] = "<line x1='8' y1='6' x2='21' y2='6'/><line x1='8' y1='12' x2='21' y2='12'/><line x1='8' y1='18' x2='21' y2='18'/><line x1='3' y1='6' x2='3.01' y2='6'/><line x1='3' y1='12' x2='3.01' y2='12'/><line x1='3' y1='18' x2='3.01' y2='18'/>",
        ["plus"] = "<line x1='12' y1='5' x2='12' y2='19'/><line x1='5' y1='12' x2='19' y2='12'/>",
        ["inbox"] = "<polyline points='22 12 16 12 14 15 10 15 8 12 2 12'/><path d='M5.45 5.11L2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z'/>",
        ["user"] = "<path d='M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2'/><circle cx='12' cy='7' r='4'/>",
        ["mic"] = "<path d='M12 1a3 3 0 0 0-3 3v8a3 3 0 0 0 6 0V4a3 3 0 0 0-3-3z'/><path d='M19 10v2a7 7 0 0 1-14 0v-2'/><line x1='12' y1='19' x2='12' y2='23'/>",
        ["mic-off"] = "<path d='M12 1a3 3 0 0 0-3 3v8a3 3 0 0 0 6 0V4a3 3 0 0 0-3-3z'/><path d='M19 10v2a7 7 0 0 1-14 0v-2'/><line x1='1' y1='1' x2='23' y2='23'/>",
        ["clock"] = "<circle cx='12' cy='12' r='10'/><polyline points='12 6 12 12 16 14'/>",
        ["lock"] = "<rect x='3' y='11' width='18' height='11' rx='2'/><path d='M7 11V7a5 5 0 0 1 10 0v4'/>",
        ["check"] = "<polyline points='20 6 9 17 4 12'/>",
        ["x"] = "<line x1='18' y1='6' x2='6' y2='18'/><line x1='6' y1='6' x2='18' y2='18'/>",
        ["logout"] = "<path d='M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4'/><polyline points='16 17 21 12 16 7'/><line x1='21' y1='12' x2='9' y2='12'/>",
        ["link"] = "<path d='M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71'/><path d='M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71'/>",
        ["arrow"] = "<line x1='5' y1='12' x2='19' y2='12'/><polyline points='12 5 19 12 12 19'/>",
        ["trash"] = "<polyline points='3 6 5 6 21 6'/><path d='M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6'/><path d='M10 11v6M14 11v6'/>"
    };

    public static HtmlString Get(string name) => new HtmlString(
        "<svg class='ic' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true'>" +
        (Paths.TryGetValue(name, out var p) ? p : "") + "</svg>");
}
