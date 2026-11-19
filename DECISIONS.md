# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

Am ales R7, cea cu suprapunerea. In starter era in `Services/BookingService.cs`, acum e in `Room.Book` din `Domain/Rooms/Room.cs`, iar formula e in `DateRange.Overlaps`. `Booking.Create` e `internal`, deci o rezervare se face doar prin camera si nimeni nu mai poate ocoli regula. Handler-ul doar incarca datele si apeleaza `room.Book(...)`.

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Camera ocupata e un caz normla, nu o exceptie, iar cu `Result<T>` se vede din semnatura ca metoda poate pica. `Room.Book` intoarce `BookingErrors.RoomOccupied`, iar `CreateBookingHandler` intoarce eroarea mai departe fara `SaveChangesAsync`, deci nu se scrie nimic in baza de date. Apoi endpoint-ul o tranforma cu `ToProblem()` intr-un 409 problem+json.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

EF Core il stiu clasele din `Infrastructure/Persistence` (`RoomHubDbContext`, configuratiile, repository-urile), `Program.cs` si endpoint-urile GET care citesc cu `AsNoTracking`. `Domain/` si handler-ele nu stiu de el, au doar interfetele `IRoomRepository`, `IBookingRepository` si `IUnitOfWork`. Castig teste fara baza de date si pot schimba persitenta usor, iar platesc cu mai multe fisiere si un strat in plus.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

```
POST /api/rooms/{roomId}/bookings -> CreateBookingEndpoint.Handle (Features/Bookings/CreateBooking)
  -> CreateBookingHandler -> RoomRepository.GetByIdAsync -> BookingRepository.ListActiveByRoomAsync
  -> Room.Book (Booking.Create, DateRange.Overlaps) -> BookingRepository.Add
  -> RoomHubDbContext.SaveChangesAsync -> roomhub.db
  eroare: Error -> ToProblem() (Common/ErrorHttpExtensions.cs) -> 400/404/409
```

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

AI-ul a propus `UseExceptionHandler()`, dar l-am scos pentru ca in Development transforma erorile de binding din 400 in 500. In `Room.Book` variabila `booking` era de fapt un `Result<Booking>`, deci `return booking;` parea ca intoarce entitatea, asa ca am cerut `bookingResult`. Am refuzat si `ExistsOverlapAsync` in repository, ca ar fi dus regula de suprapunere in infrastructura.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

Riscul ramas e race-ul de tip check-then-act, la 40 de rezervari trimise deodata 2 au trecut pe acelasi interval, iar acum se aplica si la limita de 2 pe zi. Cu 30 de minute in plus as incerca o tranzactie in SQLite sau un concurrency token, dar nu erau in cursurile 1-6. Mai as adauga un index pe `BookedBy` si `StartUtc` si migratii reale in loc de `EnsureCreated`.
