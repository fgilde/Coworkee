using System.Globalization;
using Coworkee.Shared.Constants.Localization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Coworkee.Shared;

public partial class EmailTemplate
{
    public string Content { get; private set; }
    public CultureInfo Culture { get; private set; }
    public string Name { get; set; }
    public bool IncludeLayout { get; set; } = true;

    private EmailTemplate(string name, string content, CultureInfo culture)
    {
        Name = name;
        Content = content;
        Culture = culture;
    }

    public EmailTemplate WithLayout()
    {
        IncludeLayout = true;
        return this;
    }

    public EmailTemplate WithoutLayout()
    {
        IncludeLayout = false;
        return this;
    }

    public static EmailTemplate Get(string templateName, string cultureName)
    {
        templateName = templateName.EndsWith(".cshtml") ? templateName : $"{templateName}.cshtml";
        cultureName ??= CultureInfo.CurrentCulture.Name;
        var assembly = typeof(EmailTemplate).Assembly;
        var assemblyName = assembly.GetName().Name;
        CultureInfo usedCulture = null;

        string[] culturesToTry = {
            cultureName.Replace("-", "_"),
            LocalizationConstants.DefaultLanguageCode.Replace("-", "_"), // "en_US"
            null // indicates to try without culture
        };

        var resourceName = culturesToTry
            .Select(culture => {
                if (culture != null) usedCulture = new CultureInfo(culture.Replace("_", "-"));
                return BuildResourceName(assemblyName, culture, templateName);
            })
            .FirstOrDefault(name => LocalizationConstants.GetEmbeddedResourceNames().Contains(name));

        if (resourceName != null)
            return new EmailTemplate(templateName, ReadResource(assembly, resourceName), usedCulture ?? CultureInfo.CurrentCulture);

        throw new FileNotFoundException($"Resource for template '{templateName}' not found.");
    }

    public static EmailTemplate Get(string templateName, CultureInfo culture = null) => Get(templateName, culture?.Name);

    private static string BuildResourceName(string assemblyName, string cultureName, string templateName) =>
        cultureName switch
        {
            null => LocalizationConstants.GetEmbeddedResourceNames().FirstOrDefault(r => r.EndsWith(templateName)),
            _ => $"{assemblyName}.Resources.Emails.{cultureName}.{templateName}"
        };

    private static string ReadResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }


}