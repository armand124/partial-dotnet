using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public sealed class Room
{
    public const int MaxNameLength = 100;

    private Room() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    public static Result<Room> Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.InvalidName;

        if (capacity <= 0)
            return RoomErrors.InvalidCapacity;

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };
    }

    public Result<Booking> Book(
        string? bookedBy,
        DateTime start,
        DateTime end,
        int attendees,
        IReadOnlyCollection<Booking> existingBookings,
        DateTime nowUtc)
    {
        var bookingResult = Booking.Create(Id, bookedBy, start, end, attendees, nowUtc);
        if (bookingResult.IsFailure)
            return bookingResult.Error;

        var booking = bookingResult.Value;

        if (booking.Attendees > Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        var occupied = existingBookings.Any(existing =>
            existing.RoomId == Id
            && existing.Status == BookingStatus.Active
            && existing.Range.Overlaps(booking.Range));
        if (occupied)
            return BookingErrors.RoomOccupied;

        return booking;
    }
}
