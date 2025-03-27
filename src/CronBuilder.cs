using Soenneker.Cron.Builder.Abstract;
using System;
using Soenneker.Enums.DayOfWeek;
using Soenneker.Extensions.String;

namespace Soenneker.Cron.Builder;

///<inheritdoc cref="ICronBuilder"/>
public class CronBuilder : ICronBuilder
{
    private string? _second;
    private string _minute = "*";
    private string _hour = "*";
    private string _dayOfMonth = "*";
    private string _month = "*";
    private string _dayOfWeek = "*";

    public ICronBuilder WithSeconds(int second)
    {
        if (second is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(second), "Seconds must be between 0 and 59.");

        _second = second.ToString();
        return this;
    }

    public ICronBuilder WithMinute(int minute)
    {
        if (minute is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(minute), "Minutes must be between 0 and 59.");

        _minute = minute.ToString();
        return this;
    }

    public ICronBuilder WithHour(int hour)
    {
        if (hour is < 0 or > 23)
            throw new ArgumentOutOfRangeException(nameof(hour), "Hours must be between 0 and 23.");

        _hour = hour.ToString();
        return this;
    }

    public ICronBuilder OnDayOfMonth(int day)
    {
        if (day is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(day), "Day of month must be between 1 and 31.");

        _dayOfMonth = day.ToString();
        return this;
    }

    public ICronBuilder OnMonth(int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");

        _month = month.ToString();
        return this;
    }

    public ICronBuilder OnDayOfWeek(DayOfWeekType day)
    {
        _dayOfWeek = day.ToString().ToUpperInvariantFast()[..3]; // MON, TUE, etc.
        return this;
    }

    public ICronBuilder OnDayOfWeek(string cronDayOfWeek)
    {
        _dayOfWeek = cronDayOfWeek.ToUpperInvariantFast();
        return this;
    }

    public string Build()
    {
        if (_second != null)
            return $"{_second} {_minute} {_hour} {_dayOfMonth} {_month} {_dayOfWeek}";

        return $"{_minute} {_hour} {_dayOfMonth} {_month} {_dayOfWeek}";
    }

    public override string ToString() => Build();
}