using System.Collections.Concurrent;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure;

public sealed class InMemoryRoomStore
{
    private readonly ConcurrentDictionary<Guid, Room> _rooms = new();

    public void Add(Room room) => _rooms[room.Id] = room;

    public Room? Find(Guid id) => _rooms.GetValueOrDefault(id);
}
