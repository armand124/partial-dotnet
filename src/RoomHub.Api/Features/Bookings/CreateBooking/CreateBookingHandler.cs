using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<CreateBookingResult>;

public sealed record CreateBookingResult(Guid Id, Guid RoomId, BookingStatus Status);

public sealed class CreateBookingHandler(
    InMemoryRoomStore rooms,
    InMemoryBookingStore bookings,
    TimeProvider time) : ICommandHandler<CreateBookingCommand, CreateBookingResult>
{
    public Task<Result<CreateBookingResult>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken) =>
        Task.FromResult(Execute(command));

    private Result<CreateBookingResult> Execute(CreateBookingCommand command)
    {
        var room = rooms.Find(command.RoomId);
        if (room is null)
            return RoomErrors.NotFound;

        var existing = bookings.ListByRoom(room.Id);

        var bookingResult = room.Book(
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            existing,
            time.GetUtcNow().UtcDateTime);
        if (bookingResult.IsFailure)
            return bookingResult.Error;

        var booking = bookingResult.Value;
        bookings.Add(booking);

        return new CreateBookingResult(booking.Id, booking.RoomId, booking.Status);
    }
}
