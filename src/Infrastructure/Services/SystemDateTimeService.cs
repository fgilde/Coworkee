using System;
using Coworkee.Application.Contracts.Attributes;
using Coworkee.Application.Contracts.Services;

namespace Coworkee.Infrastructure.Services
{
    [RegisterAs(typeof(IDateTimeService))]
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}