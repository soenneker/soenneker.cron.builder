[![](https://img.shields.io/nuget/v/soenneker.cron.builder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cron.builder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cron.builder/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cron.builder/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cron.builder.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cron.builder/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cron.builder/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cron.builder/actions/workflows/codeql.yml)

# Soenneker.Cron.Builder

A fluent interface for building cron expressions with support for seconds, intervals, daily/weekly/monthly scheduling, and validation.

## Install

```bash
dotnet add package Soenneker.Cron.Builder
```

## Quick start

```csharp
using Soenneker.Cron.Builder.Abstract;

ICronBuilder cronBuilder = /* resolve from DI */;
var result = cronBuilder.WithSeconds(1);
```

Sets the second (0-59) when the job should run.

## What you get

- `ICronBuilder` — A fluent interface for building cron expressions with support for seconds, intervals, daily/weekly/monthly scheduling, and validation.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `ICronBuilder.WithSeconds(second)` | Sets the second (0-59) when the job should run. | The current cron builder instance. |
| `ICronBuilder.WithMinute(minute)` | Sets the minute (0-59) when the job should run. | The current cron builder instance. |
| `ICronBuilder.WithMinuteInterval(interval)` | Sets a recurring interval in minutes (e.g., every 5 minutes = 5). | The current cron builder instance. |
| `ICronBuilder.Minutely()` | Configures the job to run every minute (minute = "*"). | The current cron builder instance. |
| `ICronBuilder.WithHour(hour)` | Sets the hour (0-23) when the job should run. | The current cron builder instance. |
| `ICronBuilder.WithHourInterval(interval)` | Sets a recurring interval in hours (e.g., every 4 hours = 4). | The current cron builder instance. |
| `ICronBuilder.Hourly()` | Configures the job to run hourly, at the top of each hour (minute = 0). | The current cron builder instance. |
| `ICronBuilder.Daily(hour, minute)` | Configures the job to run daily at a specific time. | The current cron builder instance. |
| `ICronBuilder.OnDayOfMonth(day)` | Sets the day of the month (1–31) when the job should run. | The current cron builder instance. |
| `ICronBuilder.Weekly(day, hour, minute)` | Configures the job to run weekly on a specific day and time. | The current cron builder instance. |
| `ICronBuilder.Weekdays()` | Restricts the job to run only on weekdays (Monday through Friday). | The current cron builder instance. |
| `ICronBuilder.OnMonth(month)` | Sets the month (1–12) when the job should run. | The current cron builder instance. |
| `ICronBuilder.Monthly(dayOfMonth, hour, minute)` | Configures the job to run monthly on a specific day and time. | The current cron builder instance. |
| `ICronBuilder.OnDayOfWeek(day)` | Sets the job to run on a specific day of the week using an enum. | The current cron builder instance. |
| `ICronBuilder.OnDayOfWeek(cronDayOfWeek)` | Sets the job to run on a specific day of the week using a raw cron string. | The current cron builder instance. |
| `ICronBuilder.Annually(month, dayOfMonth, hour, minute)` | Configures the job to run annually on a specific month, day, and time. | The current cron builder instance. |
| `ICronBuilder.Build()` | Builds and returns the final cron expression string. | A valid cron expression. |

## Important behavior

- `ICronBuilder.WithSeconds(second)`: Thrown if `second` is outside the valid range (0–59).
- `ICronBuilder.WithMinute(minute)`: Thrown if `minute` is outside the valid range (0–59).
- `ICronBuilder.WithMinuteInterval(interval)`: Thrown if `interval` is outside the valid range (1–59).
- `ICronBuilder.WithHour(hour)`: Thrown if `hour` is outside the valid range (0–23).
- `ICronBuilder.WithHourInterval(interval)`: Thrown if `interval` is outside the valid range (1–23).
- `ICronBuilder.OnDayOfMonth(day)`: Thrown if `day` is outside the valid range (1–31).
- `ICronBuilder.OnMonth(month)`: Thrown if `month` is outside the valid range (1–12).
- `ICronBuilder.Build()`: Thrown if both day-of-month and day-of-week are set, which may result in ambiguous scheduling behavior.
