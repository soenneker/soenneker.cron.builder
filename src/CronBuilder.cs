using Soenneker.Cron.Builder.Abstract;
using Soenneker.Enums.DayOfWeek;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Soenneker.Cron.Builder;

/// <inheritdoc cref="ICronBuilder"/>
public sealed class CronBuilder : ICronBuilder
{
    private bool _hasSeconds;
    private byte _second;

    private enum MinuteMode : byte { Fixed0, Any, Fixed, Interval }
    private MinuteMode _minuteMode = MinuteMode.Fixed0;
    private byte _minute; // Fixed / Interval

    private enum HourMode : byte { Any, Fixed, Interval }
    private HourMode _hourMode = HourMode.Any;
    private byte _hour; // Fixed / Interval

    private enum DomMode : byte { Any, Fixed }
    private DomMode _domMode = DomMode.Any;
    private byte _dayOfMonth;

    private enum MonthMode : byte { Any, Fixed }
    private MonthMode _monthMode = MonthMode.Any;
    private byte _month;

    private enum DowMode : byte { Any, Weekdays, FixedToken, Custom }
    private DowMode _dowMode = DowMode.Any;

    private DayOfWeekType _dowValue;
    private string? _customDowUpper;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowRange(string name, string msg) => throw new ArgumentOutOfRangeException(name, msg);

    public ICronBuilder WithSeconds(int second)
    {
        if ((uint)second > 59u) ThrowRange(nameof(second), "Seconds must be between 0 and 59.");
        _hasSeconds = true;
        _second = (byte)second;
        return this;
    }

    public ICronBuilder WithMinute(int minute)
    {
        if ((uint)minute > 59u) ThrowRange(nameof(minute), "Minutes must be between 0 and 59.");
        _minuteMode = MinuteMode.Fixed;
        _minute = (byte)minute;
        return this;
    }

    public ICronBuilder WithMinuteInterval(int interval)
    {
        if ((uint)(interval - 1) > 58u) ThrowRange(nameof(interval), "Interval must be between 1 and 59.");
        _minuteMode = MinuteMode.Interval;
        _minute = (byte)interval;
        return this;
    }

    public ICronBuilder Minutely()
    {
        _minuteMode = MinuteMode.Any;
        return this;
    }

    public ICronBuilder WithHour(int hour)
    {
        if ((uint)hour > 23u) ThrowRange(nameof(hour), "Hours must be between 0 and 23.");
        _hourMode = HourMode.Fixed;
        _hour = (byte)hour;
        return this;
    }

    public ICronBuilder WithHourInterval(int interval)
    {
        if ((uint)(interval - 1) > 22u) ThrowRange(nameof(interval), "Interval must be between 1 and 23.");
        _hourMode = HourMode.Interval;
        _hour = (byte)interval;
        return this;
    }

    public ICronBuilder Hourly()
    {
        _minuteMode = MinuteMode.Fixed0;
        _hourMode = HourMode.Any;
        return this;
    }

    public ICronBuilder Daily(int hour = 0, int minute = 0) => WithHour(hour).WithMinute(minute);

    public ICronBuilder OnDayOfMonth(int day)
    {
        if ((uint)(day - 1) > 30u) ThrowRange(nameof(day), "Day of month must be between 1 and 31.");
        _domMode = DomMode.Fixed;
        _dayOfMonth = (byte)day;
        return this;
    }

    public ICronBuilder Weekly(DayOfWeekType day, int hour = 0, int minute = 0) =>
        WithHour(hour).WithMinute(minute).OnDayOfWeek(day);

    public ICronBuilder Weekdays()
    {
        _dowMode = DowMode.Weekdays;
        _customDowUpper = null;
        return this;
    }

    public ICronBuilder OnMonth(int month)
    {
        if ((uint)(month - 1) > 11u) ThrowRange(nameof(month), "Month must be between 1 and 12.");
        _monthMode = MonthMode.Fixed;
        _month = (byte)month;
        return this;
    }

    public ICronBuilder Monthly(int dayOfMonth, int hour = 0, int minute = 0) =>
        OnDayOfMonth(dayOfMonth).WithHour(hour).WithMinute(minute);

    public ICronBuilder OnDayOfWeek(DayOfWeekType day)
    {
        _dowMode = DowMode.FixedToken;
        _dowValue = day;
        _customDowUpper = null;
        return this;
    }

    public ICronBuilder OnDayOfWeek(string cronDayOfWeek)
    {
        if (cronDayOfWeek is null) throw new ArgumentNullException(nameof(cronDayOfWeek));
        _customDowUpper = cronDayOfWeek.ToUpperInvariant(); // one-time allocation, reused
        _dowMode = DowMode.Custom;
        return this;
    }

    public ICronBuilder Annually(int month, int dayOfMonth, int hour = 0, int minute = 0) =>
        OnMonth(month).OnDayOfMonth(dayOfMonth).WithHour(hour).WithMinute(minute);

    public string Build()
    {
        // Cron ambiguity rule
        if (_domMode == DomMode.Fixed && _dowMode != DowMode.Any)
            throw new InvalidOperationException("Specifying both day-of-month and day-of-week is not allowed; it can lead to ambiguous cron behavior.");

        // Compute exact output length to allocate once.
        int fieldCount = _hasSeconds ? 6 : 5;
        int len = (fieldCount - 1); // spaces

        if (_hasSeconds) len += FieldLen_Byte(_second);

        len += FieldLen_Minute(_minuteMode, _minute);
        len += FieldLen_Hour(_hourMode, _hour);
        len += FieldLen_DayOfMonth(_domMode, _dayOfMonth);
        len += FieldLen_Month(_monthMode, _month);
        len += FieldLen_DayOfWeek(_dowMode, _dowValue, _customDowUpper);

        return string.Create(len, this, static (span, b) =>
        {
            int i = 0;

            if (b._hasSeconds)
            {
                i += WriteField_Byte(span.Slice(i), b._second);
                span[i++] = ' ';
            }

            i += WriteField_Minute(span.Slice(i), b._minuteMode, b._minute);
            span[i++] = ' ';

            i += WriteField_Hour(span.Slice(i), b._hourMode, b._hour);
            span[i++] = ' ';

            i += WriteField_DayOfMonth(span.Slice(i), b._domMode, b._dayOfMonth);
            span[i++] = ' ';

            i += WriteField_Month(span.Slice(i), b._monthMode, b._month);
            span[i++] = ' ';

            i += WriteField_DayOfWeek(span.Slice(i), b._dowMode, b._dowValue, b._customDowUpper);

            // i should equal span.Length; if not, logic error.
        });
    }

    public override string ToString() => Build();

    // -------------------
    // Length calculations
    // -------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DigitLen(byte v) => v >= 10 ? 2 : 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FieldLen_Byte(byte v) => DigitLen(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FieldLen_Minute(MinuteMode mode, byte v) => mode switch
    {
        MinuteMode.Fixed0 => 1,           // "0"
        MinuteMode.Any => 1,              // "*"
        MinuteMode.Fixed => DigitLen(v),  // "5" / "15"
        MinuteMode.Interval => 2 + 1 + DigitLen(v), // "*/" + digits
        _ => 1
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FieldLen_Hour(HourMode mode, byte v) => mode switch
    {
        HourMode.Any => 1,
        HourMode.Fixed => DigitLen(v),
        HourMode.Interval => 2 + 1 + DigitLen(v), // "*/" + digits
        _ => 1
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FieldLen_DayOfMonth(DomMode mode, byte v) => mode == DomMode.Any ? 1 : DigitLen(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FieldLen_Month(MonthMode mode, byte v) => mode == MonthMode.Any ? 1 : DigitLen(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FieldLen_DayOfWeek(DowMode mode, DayOfWeekType day, string? customUpper) => mode switch
    {
        DowMode.Any => 1,
        DowMode.Weekdays => 7, // "MON-FRI"
        DowMode.FixedToken => 3,
        DowMode.Custom => customUpper?.Length ?? 1,
        _ => 1
    };

    // -------------------
    // Writers (span fill)
    // -------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteField_Byte(Span<char> dst, byte v)
    {
        // v is small (0-59 / etc)
        if (v >= 10)
        {
            dst[0] = (char)('0' + (v / 10));
            dst[1] = (char)('0' + (v % 10));
            return 2;
        }

        dst[0] = (char)('0' + v);
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteStar(Span<char> dst)
    {
        dst[0] = '*';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteZero(Span<char> dst)
    {
        dst[0] = '0';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteInterval(Span<char> dst, byte v)
    {
        dst[0] = '*';
        dst[1] = '/';
        return 2 + WriteField_Byte(dst.Slice(2), v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteField_Minute(Span<char> dst, MinuteMode mode, byte v) => mode switch
    {
        MinuteMode.Fixed0 => WriteZero(dst),
        MinuteMode.Any => WriteStar(dst),
        MinuteMode.Fixed => WriteField_Byte(dst, v),
        MinuteMode.Interval => WriteInterval(dst, v),
        _ => WriteZero(dst)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteField_Hour(Span<char> dst, HourMode mode, byte v) => mode switch
    {
        HourMode.Any => WriteStar(dst),
        HourMode.Fixed => WriteField_Byte(dst, v),
        HourMode.Interval => WriteInterval(dst, v),
        _ => WriteStar(dst)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteField_DayOfMonth(Span<char> dst, DomMode mode, byte v) =>
        mode == DomMode.Any ? WriteStar(dst) : WriteField_Byte(dst, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteField_Month(Span<char> dst, MonthMode mode, byte v) =>
        mode == MonthMode.Any ? WriteStar(dst) : WriteField_Byte(dst, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteField_DayOfWeek(Span<char> dst, DowMode mode, DayOfWeekType day, string? customUpper)
    {
        switch (mode)
        {
            case DowMode.Any:
                dst[0] = '*';
                return 1;

            case DowMode.Weekdays:
                // "MON-FRI"
                dst[0] = 'M'; dst[1] = 'O'; dst[2] = 'N'; dst[3] = '-'; dst[4] = 'F'; dst[5] = 'R'; dst[6] = 'I';
                return 7;

            case DowMode.FixedToken:
                {
                    var token = GetDowToken(day); // 3-char string constant
                    dst[0] = token[0];
                    dst[1] = token[1];
                    dst[2] = token[2];
                    return 3;
                }

            case DowMode.Custom:
                if (customUpper is null)
                {
                    dst[0] = '*';
                    return 1;
                }

                customUpper.AsSpan().CopyTo(dst);
                return customUpper.Length;

            default:
                dst[0] = '*';
                return 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetDowToken(DayOfWeekType day) => day.Value switch
    {
        DayOfWeekType.MondayValue => "MON",
        DayOfWeekType.TuesdayValue => "TUE",
        DayOfWeekType.WednesdayValue => "WED",
        DayOfWeekType.ThursdayValue => "THU",
        DayOfWeekType.FridayValue => "FRI",
        DayOfWeekType.SaturdayValue => "SAT",
        DayOfWeekType.SundayValue => "SUN",
        _ => throw new ArgumentOutOfRangeException(nameof(day))
    };
}