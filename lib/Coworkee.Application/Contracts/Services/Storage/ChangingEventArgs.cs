using System.Diagnostics.CodeAnalysis;

namespace lib.Coworkee.Application.Contracts.Services.Storage;

[ExcludeFromCodeCoverage]
public class ChangingEventArgs : ChangedEventArgs
{
    public bool Cancel { get; set; }
}