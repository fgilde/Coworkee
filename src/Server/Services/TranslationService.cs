using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Application.Configurations;
using lib.Coworkee.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Attributes;
using TranslatorService;
using TranslatorService.Models.Translation;

namespace Coworkee.Server.Services;

[RegisterAs(typeof(ITranslationService), ServiceLifetime = ServiceLifetime.Scoped)]
public class TranslationService: ITranslationService
{
    private readonly ServerConfiguration _configuration;

    public TranslationService(ServerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IDictionary<string, IDictionary<string, string>>> TranslateResourceDictionaryAsync(IDictionary<string, string> input, params string[] targets)
    {
        var result = await TranslateAsync(input.Values, targets);
        var byTarget = new Dictionary<string, IDictionary<string, string>>();
        for (var cultureIndex = 0; cultureIndex < targets.Length; cultureIndex++)
        {
            var targetCulture = targets[cultureIndex];
            var resultDictionary = new Dictionary<string, string>();
            int keyIndex = 0;
            foreach (var pair in input)
            {
                if (!resultDictionary.ContainsKey(pair.Key))
                {
                    var translated = result[keyIndex]?.Translations?.ToList()[cultureIndex]?.Text;
                    if (!string.IsNullOrWhiteSpace(translated))
                    {
                        resultDictionary[pair.Key] = translated;
                    }
                }

                keyIndex++;
            }
            byTarget.Add(targetCulture, resultDictionary);
        }

        return byTarget;
    }

    private TranslatorClient CreateClient()
    {
        return new TranslatorClient(_configuration.CognitiveServices.Translation.Key, _configuration.CognitiveServices.Translation.Region);
    }

    private async Task<IList<TranslationResponse>> TranslateAsync(IEnumerable<string> inputs, string[] target)
    {
        var client = CreateClient();
        var result = new List<TranslationResponse>();
        foreach (var list in inputs.Chunk(25))
        {
            var toAdd = await client.TranslateAsync(list, target);
            result.AddRange(toAdd);
        }

        return result;
    }
}