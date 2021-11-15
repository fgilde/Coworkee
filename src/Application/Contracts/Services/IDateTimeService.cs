using System;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}