using System;
using lib.Coworkee.Application.Contracts.Services;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Infrastructure.Services
{
    [RegisterAs(typeof(IDateTimeService))]
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}