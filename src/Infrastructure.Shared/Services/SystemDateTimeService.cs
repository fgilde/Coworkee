using CleanArchitectureBase.Application.Interfaces.Services;
using System;

namespace CleanArchitectureBase.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}