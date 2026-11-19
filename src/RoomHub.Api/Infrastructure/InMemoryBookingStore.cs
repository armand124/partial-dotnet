using System.Collections.Concurrent;

namespace RoomHub.Api.Infrastructure;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed record BookingRecord(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    BookingStatus Status);

public sealed class InMemoryBookingStore
{
    private readonly ConcurrentDictionary<Guid, BookingRecord> _bookings = new();

    public void Add(BookingRecord record) => _bookings[record.Id] = record;

    public BookingRecord? Find(Guid id) => _bookings.GetValueOrDefault(id);

    public void Update(BookingRecord record) => _bookings[record.Id] = record;

    public IReadOnlyList<BookingRecord> ListByRoom(Guid roomId) =>
        _bookings.Values.Where(b => b.RoomId == roomId).ToList();
}
