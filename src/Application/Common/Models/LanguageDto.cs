using System.Globalization;
using CleanArchitectureBase.Shared.Constants.Localization;

namespace CleanArchitectureBase.Application.Common.Models;

public class LanguageDto: LanguageCode, IDtoBase<int>
{
    public bool IsNew => Id == 0;
    public int Id { get; set; }
    public bool IsActive { get; set; }
}