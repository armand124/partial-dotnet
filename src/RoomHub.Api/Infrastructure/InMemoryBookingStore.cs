using System.Collections.Concurrent;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure;

public sealed class InMemoryBookingStore
{
    private readonly ConcurrentDictionary<Guid, Booking> _bookings = new();

    public void Add(Booking booking) => _bookings[booking.Id] = booking;

    public Booking? Find(Guid id) => _bookings.GetValueOrDefault(id);

    public IReadOnlyList<Booking> ListByRoom(Guid roomId) =>
        _bookings.Values.Where(b => b.RoomId == roomId).ToList();
}
