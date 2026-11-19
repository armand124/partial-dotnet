namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Booking>> ListActiveByRoomAsync(Guid roomId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Booking>> ListActiveByBookedByOnDayAsync(
        string bookedBy,
        DateTime dayStartUtc,
        CancellationToken cancellationToken);

    void Add(Booking booking);
}
