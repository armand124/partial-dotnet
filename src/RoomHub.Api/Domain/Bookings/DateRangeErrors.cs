using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class DateRangeErrors
{
    public static readonly Error EndBeforeStart = Error.Validation(
        "InvalidRange", "The end must be after the start.");
}
