using System;

namespace Soenneker.Cron.Builder.Tests;

public sealed class CronBuilderIntervalTests
{
    [Test]
    public void IntervalExpressionsHaveExactLengthWithoutTrailingNulls()
    {
        foreach (bool includeSeconds in new[] { false, true })
        {
            for (var minute = 0; minute <= 59; minute++)
            {
                for (var hour = 0; hour <= 23; hour++)
                {
                    var builder = new CronBuilder();
                    if (includeSeconds)
                        builder.WithSeconds(17);
                    if (minute != 0)
                        builder.WithMinuteInterval(minute);
                    if (hour != 0)
                        builder.WithHourInterval(hour);

                    string expected = (includeSeconds ? "17 " : "") +
                                      (minute == 0 ? "0" : $"*/{minute}") + " " +
                                      (hour == 0 ? "*" : $"*/{hour}") + " * * *";

                    if (builder.Build() != expected)
                        throw new InvalidOperationException($"Incorrect expression for minute interval {minute}, hour interval {hour}, seconds {includeSeconds}.");
                }
            }
        }
    }
}
