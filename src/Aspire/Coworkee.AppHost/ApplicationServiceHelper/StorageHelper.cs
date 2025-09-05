using Aspire.Hosting.Azure;
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.Extensions.Hosting;

namespace Coworkee.AppHost.ApplicationServiceHelper;

internal static class StorageHelper
{
    public static IEnumerable<IResourceBuilder<IResource>> AddAzureStorageIf(this IDistributedApplicationBuilder builder, bool condition)
    {
        if(!condition)
            yield break;
        var res = AddAzureStorage(builder, out var blobs);
        yield return res;
        yield return blobs;
    }

    public static IResourceBuilder<AzureStorageResource> AddAzureStorage(this IDistributedApplicationBuilder builder, out IResourceBuilder<AzureBlobStorageResource> blobs)
    {
        var storage = builder.AddAzureStorage(ApplicationConstants.ServiceNames.Storage);
        //.ConfigureInfrastructure(infra =>
        //{
        //    var storageAccount = infra.GetProvisionableResources()
        //        .OfType<StorageAccount>()
        //        .Single();

        //    storageAccount.Kind = StorageKind.StorageV2;
        //    storageAccount.AccessTier = StorageAccountAccessTier.Hot;
        //    storageAccount.Sku = new StorageSku { Name = StorageSkuName.StandardLrs };
        //    //storageAccount.Tags.Add("ExampleKey", "Example value");
        //})
        //;

        if (builder.Environment.IsDevelopment() && builder.ExecutionContext.IsRunMode)
        {
            storage.RunAsEmulator(azurite =>
            {
                azurite.WithBlobPort(27000)
                    .WithQueuePort(27001)
                    .WithTablePort(27002)
                    .WithDataVolume()
                    //  .WithDataBindMount("../Azurite/Data")
                    .WithLifetime(ContainerLifetime.Persistent);
            });
        }


        blobs = storage.AddBlobs(ApplicationConstants.ServiceNames.Blobs);
        return storage;
    }
}