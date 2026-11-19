using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/{id:guid}/cancel", Handle).WithName("CancelBooking");

    private static Results<NoContent, ProblemHttpResult> Handle(
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

        if (booking.Status == BookingStatus.Cancelled)
        {
            return TypedResults.Problem(
                title: "BookingAlreadyCancelled",
                detail: "The booking is already cancelled.",
                statusCode: StatusCodes.Status409Conflict);
        }

        store.Update(booking with { Status = BookingStatus.Cancelled });
        return TypedResults.NoContent();
    }
}
