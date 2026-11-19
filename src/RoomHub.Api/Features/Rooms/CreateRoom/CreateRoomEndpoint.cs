using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public sealed record Request(string? Name, int Capacity);

    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/", Handle).WithName("CreateRoom");

    private static Results<Created<Response>, ProblemHttpResult> Handle(
        Request request,
        InMemoryRoomStore store)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.Problem(
                title: "InvalidName",
                detail: "Room name is required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (request.Capacity <= 0)
        {
            return TypedResults.Problem(
                title: "InvalidCapacity",
                detail: "Capacity must be positive.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var record = new RoomRecord(Guid.NewGuid(), request.Name.Trim(), request.Capacity);
        store.Add(record);

        var response = new Response(record.Id, record.Name, record.Capacity);
        return TypedResults.Created($"/api/rooms/{record.Id}", response);
    }
}
