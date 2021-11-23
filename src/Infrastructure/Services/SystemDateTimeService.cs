using System;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Services;

namespace CleanArchitectureBase.Infrastructure.Services
{
    [RegisterAs(typeof(IDateTimeService))]
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}