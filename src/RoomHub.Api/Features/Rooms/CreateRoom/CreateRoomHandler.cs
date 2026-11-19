using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int Capacity) : ICommand<CreateRoomResult>;

public sealed record CreateRoomResult(Guid Id, string Name, int Capacity);

public sealed class CreateRoomHandler(InMemoryRoomStore rooms)
    : ICommandHandler<CreateRoomCommand, CreateRoomResult>
{
    public Task<Result<CreateRoomResult>> Handle(
        CreateRoomCommand command,
        CancellationToken cancellationToken) =>
        Task.FromResult(Execute(command));

    private Result<CreateRoomResult> Execute(CreateRoomCommand command)
    {
        var roomResult = Room.Create(command.Name, command.Capacity);
        if (roomResult.IsFailure)
            return roomResult.Error;

        var room = roomResult.Value;
        rooms.Add(room);

        return new CreateRoomResult(room.Id, room.Name, room.Capacity);
    }
}
