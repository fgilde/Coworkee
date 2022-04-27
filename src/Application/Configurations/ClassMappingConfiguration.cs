using CleanArchitectureBase.Application.Common.Models;
using Nextended.Core.Helper;

namespace CleanArchitectureBase.Application.Configurations;

internal class ClassMappingConfiguration
{
    internal static void RegisterConverters(Idhashing hashSettings)
    {
        if (hashSettings.Enabled)
        {
            HashedInt.SetSettings(hashSettings.MinLength, hashSettings.Salt, hashSettings.AllowAccessWithNotHashedId);
            ClassMappingSettings.AddGlobalConverter<string, int>(s => new HashedInt(s).Id);
            ClassMappingSettings.AddGlobalConverter<string, int[]>(s => new HashedInt(s).Ids);
            ClassMappingSettings.AddGlobalConverter<int, string>(i => new HashedInt(i).Hash);
            ClassMappingSettings.AddGlobalConverter<int[], string>(i => new HashedInt(i).Hash);
        }
    }
}