using System.Globalization;
using Coworkee.Shared.Constants.Localization;

namespace Coworkee.Application.Common.Models;

public class LanguageDto: LanguageCode, IDtoBase<int>
{
    public LanguageDto()
    { }
    public bool IsNew => Id == 0;
    public int Id { get; set; }
    public bool IsActive { get; set; }
}