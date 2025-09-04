using System;

namespace Coworkee.Application.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterAsIfConfigValueEqualsAttribute(Type registerAsType, string requiredValue, string[] configPath)
    : RegisterIfConfigAttribute(registerAsType, configPath)
{
    public StringComparison Comparison { get; set; }

    protected virtual bool Invert => false;

    public RegisterAsIfConfigValueEqualsAttribute(Type registerAsType, string requiredValue, string configPath)
        : this(registerAsType, requiredValue, new[] { configPath })
    {}

    protected override bool IsEnabled()
    {
        var enabled = base.IsEnabled();
        if (!enabled)
            return false;

        var equals = FindValue().Equals(requiredValue, Comparison);
        return Invert ? !equals : equals;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterAsIfConfigValueNotEqualsAttribute : RegisterAsIfConfigValueEqualsAttribute
{
    public RegisterAsIfConfigValueNotEqualsAttribute(Type registerAsType, string requiredValue, string configPath) : base(registerAsType, requiredValue, configPath)
    { }

    public RegisterAsIfConfigValueNotEqualsAttribute(Type registerAsType, string requiredValue, string[] configPath) : base(registerAsType, requiredValue, configPath)
    { }

    protected override bool Invert => true;
}