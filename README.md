[![](https://img.shields.io/nuget/v/soenneker.cron.builder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cron.builder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cron.builder/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cron.builder/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cron.builder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cron.builder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cron.builder/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cron.builder/actions/workflows/codeql.yml)

# Soenneker.Cron.Builder

A mutable fluent builder for five-field cron expressions, with optional leading seconds for six-field schedulers.

## Installation

```bash
dotnet add package Soenneker.Cron.Builder
```

## Common schedules

Construct a new builder for each expression; no dependency-injection registration is required.

```csharp
using Soenneker.Cron.Builder;
using Soenneker.Enums.DayOfWeek;

string everyFiveMinutes = new CronBuilder()
    .WithMinuteInterval(5)
    .Build();
// */5 * * * *

string weekdayMorning = new CronBuilder()
    .Daily(hour: 9, minute: 30)
    .Weekdays()
    .Build();
// 30 9 * * MON-FRI

string mondayAtNoon = new CronBuilder()
    .Weekly(DayOfWeekType.Monday, hour: 12)
    .Build();
// 0 12 * * MON

string firstOfMonth = new CronBuilder()
    .Monthly(dayOfMonth: 1, hour: 2, minute: 15)
    .Build();
// 15 2 1 * *
```

`ToString()` returns the same value as `Build()`.

## Field order and defaults

Without `WithSeconds`, output uses the conventional order:

```text
minute hour day-of-month month day-of-week
```

The untouched builder produces `0 * * * *`, which means the top of every hour. `Minutely()` changes the minute field to `*`; `Hourly()` sets minute to `0` and hour to `*`.

Calling `WithSeconds` adds a leading seconds field:

```csharp
string expression = new CronBuilder()
    .WithSeconds(30)
    .Minutely()
    .Build();
// 30 * * * * *
```

Six-field cron dialects are not universal. Confirm that the target scheduler interprets the first field as seconds before using this form.

## Composition rules

Builder calls update individual fields; they do not reset unrelated fields. This allows combinations such as `Daily(...).Weekdays()`, but reusing a builder also retains earlier month or day restrictions. Create a new builder when starting a different schedule.

Numeric ranges are checked for seconds, minutes, hours, day of month, month, and interval methods. `Build()` rejects a fixed day of month combined with any day-of-week restriction because that combination has scheduler-dependent semantics.

`OnDayOfWeek(string)` uppercases and emits the supplied token unchanged. Use it only for syntax supported by the target scheduler; the builder does not validate raw expressions.

The builder only formats expressions. It does not evaluate schedules, apply a time zone, account for daylight-saving transitions, or register jobs with a scheduler. Instances are mutable and should not be shared across concurrent expression construction.
