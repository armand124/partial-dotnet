using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    public const int MaxBookedByLength = 100;

    public const int MaxActiveBookingsPerDay = 2;

    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    public static readonly TimeSpan MinCancellationNotice = TimeSpan.FromMinutes(60);

    private Booking() { }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateRange Range { get; private set; } = null!;
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    internal static Result<Booking> Create(
        Guid roomId,
        string? bookedBy,
        DateTime start,
        DateTime end,
        int attendees,
        DateTime nowUtc)
    {
        var rangeResult = DateRange.Create(start, end);
        if (rangeResult.IsFailure)
            return rangeResult.Error;

        var range = rangeResult.Value;

        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.InvalidBookedBy;

        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (range.Duration > MaxDuration)
            return BookingErrors.TooLong;

        if (range.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = bookedBy.Trim(),
            Range = range,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    public static DateTime UtcDayOf(DateTime start) => DateRange.ToUtc(start).Date;

    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (Range.StartUtc - nowUtc < MinCancellationNotice)
            return BookingErrors.CancellationTooLate;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
