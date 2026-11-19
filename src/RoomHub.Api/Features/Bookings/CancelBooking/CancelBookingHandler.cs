using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : ICommand<Unit>;

public sealed class CancelBookingHandler(InMemoryBookingStore bookings)
    : ICommandHandler<CancelBookingCommand, Unit>
{
    public Task<Result<Unit>> Handle(
        CancelBookingCommand command,
        CancellationToken cancellationToken) =>
        Task.FromResult(Execute(command));

    private Result<Unit> Execute(CancelBookingCommand command)
    {
        var booking = bookings.Find(command.BookingId);
        if (booking is null)
            return BookingErrors.NotFound;

        var result = booking.Cancel();
        if (result.IsFailure)
            return result.Error;

        return Unit.Value;
    }
}
