using System;
using Soenneker.Enums.DayOfWeek;

namespace Soenneker.Cron.Builder.Abstract;

/// <summary>
/// Defines a fluent builder interface for constructing cron expressions using either 5-field or 6-field (with seconds) format.
/// </summary>
public interface ICronBuilder
{
    /// <summary>
    /// Sets the seconds field for a 6-field cron expression (e.g., Quartz.NET).
    /// </summary>
    /// <param name="second">The second value (0-59).</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder WithSeconds(int second);

    /// <summary>
    /// Sets the minute field for the cron expression.
    /// </summary>
    /// <param name="minute">The minute value (0-59).</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder WithMinute(int minute);

    /// <summary>
    /// Sets the hour field for the cron expression.
    /// </summary>
    /// <param name="hour">The hour value (0-23).</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder WithHour(int hour);

    /// <summary>
    /// Sets the day of month field for the cron expression.
    /// </summary>
    /// <param name="day">The day of month (1-31).</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder OnDayOfMonth(int day);

    /// <summary>
    /// Sets the month field for the cron expression.
    /// </summary>
    /// <param name="month">The month value (1-12).</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder OnMonth(int month);

    /// <summary>
    /// Sets the day of week field for the cron expression using the <see cref="DayOfWeek"/> enum.
    /// </summary>
    /// <param name="day">The day of the week (e.g., DayOfWeek.Monday).</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder OnDayOfWeek(DayOfWeekType day);

    /// <summary>
    /// Sets the day of week field using a custom 3-letter cron day string (e.g., "MON", "FRI").
    /// </summary>
    /// <param name="cronDayOfWeek">The 3-letter day string in uppercase or lowercase.</param>
    /// <returns>The current builder instance.</returns>
    ICronBuilder OnDayOfWeek(string cronDayOfWeek);

    /// <summary>
    /// Builds the final cron expression string using either 5 or 6 fields depending on whether seconds were specified.
    /// </summary>
    /// <returns>The cron expression string.</returns>
    string Build();
}