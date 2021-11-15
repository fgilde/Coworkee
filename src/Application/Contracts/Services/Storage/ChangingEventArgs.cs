using System.Diagnostics.CodeAnalysis;

namespace CleanArchitectureBase.Application.Contracts.Services.Storage
{
    [ExcludeFromCodeCoverage]
    public class ChangingEventArgs : ChangedEventArgs
    {
        public bool Cancel { get; set; }
    }
}