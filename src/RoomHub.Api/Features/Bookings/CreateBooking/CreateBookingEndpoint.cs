using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public sealed record Request(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

    public sealed record Response(Guid Id, Guid RoomId, BookingStatus Status);

    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/", Handle).WithName("CreateBooking");

    private static Results<Created<Response>, ProblemHttpResult> Handle(
        Guid roomId,
        Request request,
        InMemoryRoomStore rooms,
        InMemoryBookingStore bookings)
    {
        var room = rooms.Find(roomId);
        if (room is null)
        {
            return TypedResults.Problem(
                title: "RoomNotFound",
                detail: "The room does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        if (string.IsNullOrWhiteSpace(request.BookedBy))
        {
            return TypedResults.Problem(
                title: "InvalidBookedBy",
                detail: "BookedBy is required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (request.EndUtc <= request.StartUtc)
        {
            return TypedResults.Problem(
                title: "InvalidRange",
                detail: "The end must be after the start.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (request.Attendees <= 0)
        {
            return TypedResults.Problem(
                title: "InvalidAttendees",
                detail: "Attendees must be positive.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (request.Attendees > room.Capacity)
        {
            return TypedResults.Problem(
                title: "AttendeesExceedCapacity",
                detail: "The room does not have enough seats.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var occupied = bookings.ListByRoom(roomId).Any(existing =>
            existing.Status == BookingStatus.Active
            && existing.StartUtc < request.EndUtc
            && request.StartUtc < existing.EndUtc);
        if (occupied)
        {
            return TypedResults.Problem(
                title: "RoomOccupied",
                detail: "The room is occupied in this interval.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var record = new BookingRecord(
            Guid.NewGuid(),
            roomId,
            request.BookedBy.Trim(),
            request.StartUtc,
            request.EndUtc,
            request.Attendees,
            BookingStatus.Active);
        bookings.Add(record);

        var response = new Response(record.Id, record.RoomId, record.Status);
        return TypedResults.Created($"/api/bookings/{record.Id}", response);
    }
}
