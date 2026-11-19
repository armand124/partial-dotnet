using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    public const int MaxBookedByLength = 100;

    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

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

    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
