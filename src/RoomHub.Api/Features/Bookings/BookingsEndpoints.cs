using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBookingById;

namespace RoomHub.Api.Features.Bookings;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookings(this IEndpointRouteBuilder app)
    {
        var roomBookings = app.MapGroup("/api/rooms/{roomId:guid}/bookings").WithTags("Bookings");
        CreateBookingEndpoint.Map(roomBookings);

        var bookings = app.MapGroup("/api/bookings").WithTags("Bookings");
        GetBookingByIdEndpoint.Map(bookings);
        CancelBookingEndpoint.Map(bookings);

        return app;
    }
}
