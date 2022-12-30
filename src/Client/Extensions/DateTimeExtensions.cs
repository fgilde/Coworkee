using System;

namespace Coworkee.Client.Extensions;

public static class DateTimeExtensions
{
    public static DateTime AsClientLocalTime(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
    }
}