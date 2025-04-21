using Soenneker.Enums.DayOfWeek;
using System;

namespace Soenneker.Cron.Builder.Abstract;

/// <summary>
/// A fluent interface for building cron expressions with support for seconds, intervals, daily/weekly/monthly scheduling, and validation.
/// </summary>
public interface ICronBuilder
{
    /// <summary>
    /// Sets the second (0-59) when the job should run.
    /// </summary>
    /// <param name="second">The second value (0–59).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="second"/> is outside the valid range (0–59).</exception>
    ICronBuilder WithSeconds(int second);

    /// <summary>
    /// Sets the minute (0-59) when the job should run.
    /// </summary>
    /// <param name="minute">The minute value (0–59).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="minute"/> is outside the valid range (0–59).</exception>
    ICronBuilder WithMinute(int minute);

    /// <summary>
    /// Sets a recurring interval in minutes (e.g., every 5 minutes = 5).
    /// </summary>
    /// <param name="interval">The minute interval (1–59).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="interval"/> is outside the valid range (1–59).</exception>
    ICronBuilder WithMinuteInterval(int interval);

    /// <summary>
    /// Configures the job to run every minute (minute = "*").
    /// </summary>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Minutely();

    /// <summary>
    /// Sets the hour (0-23) when the job should run.
    /// </summary>
    /// <param name="hour">The hour value (0–23).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="hour"/> is outside the valid range (0–23).</exception>
    ICronBuilder WithHour(int hour);

    /// <summary>
    /// Sets a recurring interval in hours (e.g., every 4 hours = 4).
    /// </summary>
    /// <param name="interval">The hour interval (1–23).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="interval"/> is outside the valid range (1–23).</exception>
    ICronBuilder WithHourInterval(int interval);

    /// <summary>
    /// Configures the job to run hourly, at the top of each hour (minute = 0).
    /// </summary>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Hourly();

    /// <summary>
    /// Configures the job to run daily at a specific time.
    /// </summary>
    /// <param name="hour">The hour value (0–23). Default is 0.</param>
    /// <param name="minute">The minute value (0–59). Default is 0.</param>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Daily(int hour = 0, int minute = 0);

    /// <summary>
    /// Sets the day of the month (1–31) when the job should run.
    /// </summary>
    /// <param name="day">The day of the month (1–31).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="day"/> is outside the valid range (1–31).</exception>
    ICronBuilder OnDayOfMonth(int day);

    /// <summary>
    /// Configures the job to run weekly on a specific day and time.
    /// </summary>
    /// <param name="day">The day of the week the job should run.</param>
    /// <param name="hour">The hour value (0–23). Default is 0.</param>
    /// <param name="minute">The minute value (0–59). Default is 0.</param>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Weekly(DayOfWeekType day, int hour = 0, int minute = 0);

    /// <summary>
    /// Restricts the job to run only on weekdays (Monday through Friday).
    /// </summary>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Weekdays();

    /// <summary>
    /// Sets the month (1–12) when the job should run.
    /// </summary>
    /// <param name="month">The month value (1–12).</param>
    /// <returns>The current cron builder instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="month"/> is outside the valid range (1–12).</exception>
    ICronBuilder OnMonth(int month);

    /// <summary>
    /// Configures the job to run monthly on a specific day and time.
    /// </summary>
    /// <param name="dayOfMonth">The day of the month (1–31).</param>
    /// <param name="hour">The hour value (0–23). Default is 0.</param>
    /// <param name="minute">The minute value (0–59). Default is 0.</param>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Monthly(int dayOfMonth, int hour = 0, int minute = 0);

    /// <summary>
    /// Sets the job to run on a specific day of the week using an enum.
    /// </summary>
    /// <param name="day">The day of the week the job should run (e.g., Monday).</param>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder OnDayOfWeek(DayOfWeekType day);

    /// <summary>
    /// Sets the job to run on a specific day of the week using a raw cron string.
    /// </summary>
    /// <param name="cronDayOfWeek">Three-letter cron abbreviation for the day (e.g., "MON", "TUE").</param>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder OnDayOfWeek(string cronDayOfWeek);

    /// <summary>
    /// Configures the job to run annually on a specific month, day, and time.
    /// </summary>
    /// <param name="month">Month of the year (1–12).</param>
    /// <param name="dayOfMonth">Day of the month (1–31).</param>
    /// <param name="hour">Hour of the day (0–23). Default is 0.</param>
    /// <param name="minute">Minute of the hour (0–59). Default is 0.</param>
    /// <returns>The current cron builder instance.</returns>
    ICronBuilder Annually(int month, int dayOfMonth, int hour = 0, int minute = 0);

    /// <summary>
    /// Builds and returns the final cron expression string.
    /// </summary>
    /// <returns>A valid cron expression.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if both day-of-month and day-of-week are set, which may result in ambiguous scheduling behavior.
    /// </exception>
    string Build();
}
