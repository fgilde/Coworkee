using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nextended.Core.Extensions;
using lib.Coworkee.Application.Configurations;
using Nextended.Core.Attributes;

namespace Coworkee.Application.Contracts.Attributes;

public abstract class RegisterIfConfigAttribute(Type registerAsType, string[] configPath)
    : RegisterAsAttribute(registerAsType)
{
    private readonly string _configPath = string.Join('.', configPath).Replace("::", ".");
    private static IDictionary<string, string> _config;

    public bool ThrowIfKeyInvalid { get; set; }

    protected RegisterIfConfigAttribute(Type registerAsType, string configPath)
        : this(registerAsType, [configPath])
    { }


    protected string FindValue()
    {
        _config ??= new Dictionary<string, string>(JObject.FromObject(ServerConfiguration.Instance).ToFlatDictionary(), StringComparer.OrdinalIgnoreCase);
        if (_config.TryGetValue(_configPath, out string s))
            return s;
        if (ThrowIfKeyInvalid)
            throw new ArgumentNullException($"Invalid Value for {nameof(_configPath)}. The Config path to '{_configPath}' could not be found");
        return string.Empty;
    }
}
