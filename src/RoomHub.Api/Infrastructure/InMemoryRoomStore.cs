using System.Collections.Concurrent;

namespace RoomHub.Api.Infrastructure;

public sealed record RoomRecord(Guid Id, string Name, int Capacity);

public sealed class InMemoryRoomStore
{
    private readonly ConcurrentDictionary<Guid, RoomRecord> _rooms = new();

    public void Add(RoomRecord record) => _rooms[record.Id] = record;

    public RoomRecord? Find(Guid id) => _rooms.GetValueOrDefault(id);
}
