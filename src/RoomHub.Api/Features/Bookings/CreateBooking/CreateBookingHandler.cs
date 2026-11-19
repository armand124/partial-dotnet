using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<CreateBookingResult>;

public sealed record CreateBookingResult(Guid Id, Guid RoomId, BookingStatus Status);

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider time) : ICommandHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<Result<CreateBookingResult>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var existing = await bookings.ListActiveByRoomAsync(room.Id, cancellationToken);

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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingResult(booking.Id, booking.RoomId, booking.Status);
    }
}
