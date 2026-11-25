using System.Text;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.Helpers;

public static class VCardHelper
{
    public static string ToVCard(Contact c)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCARD");
        sb.AppendLine("VERSION:3.0");
        sb.AppendLine($"FN:{Escape(c.Name)}");
        sb.AppendLine($"N:{Escape(c.Name)};;;;");

        sb.AppendLine($"TEL;TYPE=CELL:{c.PhoneNumber}");

        if (!string.IsNullOrWhiteSpace(c.Email))
            sb.AppendLine($"EMAIL:{Escape(c.Email)}");

        if (!string.IsNullOrWhiteSpace(c.Address))
            sb.AppendLine($"ADR;TYPE=HOME:;;{Escape(c.Address)};;;;");

        sb.Append("END:VCARD");

        return sb.ToString();
    }

    static string Escape(string? s) => (s ?? "")
        .Replace("\\", "\\\\").Replace(",", "\\,").Replace(";", "\\;").Replace("\n", "\\n");

}