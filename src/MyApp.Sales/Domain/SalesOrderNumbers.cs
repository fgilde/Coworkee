using System.Globalization;

namespace MyApp.Sales.Domain;

public static class SalesOrderNumbers
{
    /// <summary>"SO-" and seven digits, so the text sorts like the number.</summary>
    public static string Format(int number) => "SO-" + number.ToString("D7", CultureInfo.InvariantCulture);
}
