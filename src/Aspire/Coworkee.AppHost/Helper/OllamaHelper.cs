using Coworkee.Shared.Constants.Application;

namespace Coworkee.AppHost.Helper;

internal static class OllamaHelper
{

    public static IEnumerable<IResourceBuilder<IResource>> WithOllamaIf(this IDistributedApplicationBuilder builder, bool condition)
    {
        if (!condition)
            yield break;
        var res = WithOllama(builder);
        yield return res.OllamaResource;
        yield return res.OpenWebUIResource;
        yield return res.OllamaModelResource;
    }

    public static (IResourceBuilder<OllamaResource> OllamaResource, IResourceBuilder<OpenWebUIResource> OpenWebUIResource, IResourceBuilder<OllamaModelResource> OllamaModelResource) 
        WithOllama(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<OpenWebUIResource> openWebUi = null;

        var ollama = builder.AddOllama(ApplicationConstants.ServiceNames.Ollama)
            .WithContainerRuntimeArgs()
            .WithDataVolume()
            .WithOtlpExporter()
            .WithOpenWebUI(webui =>
            {
                openWebUi = webui;
                webui.WithOtlpExporter()
                    .WithExternalHttpEndpoints()
                    .PublishAsContainer();
            }, ApplicationConstants.ServiceNames.OllamaUI)
            .WithExternalHttpEndpoints();

        var ollamaModel = ollama.AddModel(ApplicationConstants.LargeLanguageModel);
        ollama.PublishAsContainer();

        return (ollama, openWebUi!, ollamaModel);
    }
}