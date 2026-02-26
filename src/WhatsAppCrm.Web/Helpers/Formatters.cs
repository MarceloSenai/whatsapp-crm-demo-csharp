using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WhatsAppCrm.Web.Helpers;

public static partial class Formatters
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static string FormatCurrency(double value)
    {
        return value.ToString("C", PtBr);
    }

    public static string FormatPhone(string phone)
    {
        var digits = DigitsOnly().Replace(phone, "");
        if (digits.Length == 13)
        {
            return $"+{digits[..2]} ({digits[2..4]}) {digits[4..9]}-{digits[9..]}";
        }
        return phone;
    }

    public static string TimeAgo(DateTime date)
    {
        var now = DateTime.UtcNow;
        var diff = now - date;

        if (diff.TotalMinutes < 1) return "agora";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}min";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d";
        return date.ToString("dd/MM/yyyy", PtBr);
    }

    public static string[] ParseTags(string tags)
    {
        try
        {
            return JsonSerializer.Deserialize<string[]>(tags) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public static string CssClass(params string?[] classes)
    {
        return string.Join(" ", classes.Where(c => !string.IsNullOrWhiteSpace(c)));
    }

    public static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex DigitsOnly();
}
