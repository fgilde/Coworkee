using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Common;

namespace Coworkee.Application.Contracts.Services;

public interface ITranslationService: IService
{
    /// <summary>
    /// Translate given input dictionary to given cultures and returns results grouped by culture
    /// </summary>
    /// <param name="input"></param>
    /// <param name="targetCultures"></param>
    /// <returns></returns>
    Task<IDictionary<string, IDictionary<string, string>>> TranslateResourceDictionaryAsync(
        IDictionary<string, string> input, params string[] targetCultures);
}