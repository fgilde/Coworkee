using System;

namespace lib.Coworkee.Application.Contracts.Services;

public interface IDateTimeService
{
    DateTime NowUtc { get; }
}