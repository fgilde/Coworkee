using System.Globalization;

namespace CleanArchitectureBase.Application.Common.Models;



public class LanguageDto: DtoBase<int>
{
    public bool? IsRTL { get; set; }
    public string DisplayName { get; set; }
    public string CultureCode { get; set; }
    public bool IsActive { get; set; }

    public CultureInfo ToCulture()
    {
        return new CultureInfo(CultureCode);
    }

    public static LanguageDto FromCulture(CultureInfo cultureInfo)
    {
        var displayName = cultureInfo.EnglishName;
        return new LanguageDto
        {
            DisplayName = displayName,
            CultureCode = cultureInfo.IetfLanguageTag
        };
    }
}