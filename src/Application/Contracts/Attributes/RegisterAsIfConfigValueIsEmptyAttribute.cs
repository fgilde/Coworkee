using System;

namespace CleanArchitectureBase.Application.Contracts.Attributes;

public class RegisterAsIfConfigValueIsEmptyAttribute: RegisterAsIfConfigValueEqualsAttribute
{
    public RegisterAsIfConfigValueIsEmptyAttribute(Type registerAsType, string configPath) : base(registerAsType, string.Empty, configPath)
    {}
    public RegisterAsIfConfigValueIsEmptyAttribute(Type registerAsType, string[] configPath) : base(registerAsType, string.Empty, configPath)
    { }
}

public class RegisterAsIfConfigValueIsNotEmptyAttribute : RegisterAsIfConfigValueEqualsAttribute
{
    public RegisterAsIfConfigValueIsNotEmptyAttribute(Type registerAsType, string configPath) : base(registerAsType, string.Empty, configPath)
    { }
    public RegisterAsIfConfigValueIsNotEmptyAttribute(Type registerAsType, string[] configPath) : base(registerAsType, string.Empty, configPath)
    { }

    protected override bool Invert => true;
}