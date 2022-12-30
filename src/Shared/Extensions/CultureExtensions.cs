using System.Globalization;

namespace Coworkee.Shared.Extensions;

public static class CultureExtensions
{
    public static string AcceptHeaderCode(this CultureInfo culture)
    {
        return culture?.Name;
        //return culture?.TwoLetterISOLanguageName;
    }
}