using System;
using CleanArchitectureBase.Application.Contracts.Services;

namespace CleanArchitectureBase.Infrastructure.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}