using System.Text.Json.Serialization;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddSingleton<InMemoryRoomStore>();
builder.Services.AddSingleton<InMemoryBookingStore>();

builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

app.UseStatusCodePages();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRooms();
app.MapBookings();

app.Run();
