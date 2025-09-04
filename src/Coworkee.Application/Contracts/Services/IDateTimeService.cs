using System;

namespace Coworkee.Application.Contracts.Services;

public interface IDateTimeService
{
    DateTime NowUtc { get; }
}