using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Requests;
using Nextended.Core.Helper;
using Nextended.Core.Types;

namespace Coworkee.Application.Configurations;

internal static class ClassMappingConfiguration
{
    internal static void RegisterConverters(IdHashing hashSettings)
    {
        if (hashSettings.Enabled)
        {
            HashedInt.SetSettings(hashSettings.MinLength, hashSettings.Salt, hashSettings.AllowAccessWithNotHashedId);
            ClassMappingSettings.AddGlobalConverter<string, int>(s => new HashedInt(s).Id);
            ClassMappingSettings.AddGlobalConverter<string, int[]>(s => new HashedInt(s).Ids);
            ClassMappingSettings.AddGlobalConverter<int, string>(i => new HashedInt(i).Hash);
            ClassMappingSettings.AddGlobalConverter<int[], string>(i => new HashedInt(i).Hash);
        }

        ClassMappingSettings.AddGlobalConverter<UploadRequest, DocumentDto>(r => new DocumentDto
        {
            Title = r.FileName,
            Description = $"File '{r.FileName}'",
            URL = DataUrl.GetDataUrl(r.Data),
            IsPublic = false,
            UploadRequest = r
        });
        ClassMappingSettings.AddGlobalConverter<DocumentDto, UploadRequest>(d => d.UploadRequest);
    }
}