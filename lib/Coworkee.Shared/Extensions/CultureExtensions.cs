using System.Globalization;

namespace lib.Coworkee.Shared.Extensions;

public static class CultureExtensions
{
    public static string AcceptHeaderCode(this CultureInfo culture)
    {
        return culture?.Name;
        //return culture?.TwoLetterISOLanguageName;
    }
}