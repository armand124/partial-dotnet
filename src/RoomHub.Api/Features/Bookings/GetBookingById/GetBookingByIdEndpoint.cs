using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.GetBookingById;

public static class GetBookingByIdEndpoint
{
    public sealed record Response(
        Guid Id,
        Guid RoomId,
        string BookedBy,
        DateTime StartUtc,
        DateTime EndUtc,
        int Attendees,
        BookingStatus Status);

    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle).WithName("GetBookingById");

    private static Results<Ok<Response>, ProblemHttpResult> Handle(
        Guid id,
        InMemoryBookingStore store)
    {
        var booking = store.Find(id);
        if (booking is null)
        {
            return TypedResults.Problem(
                title: "BookingNotFound",
                detail: "The booking does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(new Response(
            booking.Id,
            booking.RoomId,
            booking.BookedBy,
            booking.StartUtc,
            booking.EndUtc,
            booking.Attendees,
            booking.Status));
    }
}
