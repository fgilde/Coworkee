using System;

namespace Coworkee.Application.Contracts.Attributes;

/// <summary>
/// Just a small attribute to ensure classes are not detect as unused
/// </summary>
public class ProtectUnused: Attribute
{
    public ProtectUnused(params Type[] types)
    {}

    public ProtectUnused(params string[] names)
    { }
}