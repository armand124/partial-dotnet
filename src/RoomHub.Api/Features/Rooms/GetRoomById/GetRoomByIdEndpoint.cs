using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Rooms.GetRoomById;

public static class GetRoomByIdEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle).WithName("GetRoomById");

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(
        Guid id,
        RoomHubDbContext db,
        CancellationToken cancellationToken)
    {
        var room = await db.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound.ToProblem();

        return TypedResults.Ok(new Response(room.Id, room.Name, room.Capacity));
    }
}
