// En enkel hotellbookingmodell som viser hvordan nettbasert skjema-data kan
// transformeres til en bookingkontrakt og behandles gjennom service og repository.

enum RoomType
{
    Single,
    Double,
    Suite
}

// Grensesnitt for tjenesten som håndterer bookinger.
interface IBookingService
{
    void BookRoom(Booking booking);
    void CancelBooking(Guid bookingId);
    List<Booking> GetBookingsForCustomer(string customerName);
}

interface IValidatableBooking
{
    bool Validate(out List<string> errors);
}

// Representerer et hotell med en liste over tilgjengelige rom.
class Hotel
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required List<Room> AvailableRooms { get; set; }
}

// Et enkelt rom i hotellet.
class Room
{
    public RoomType Type { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

// Modell for en booking som er opprettet fra et kundeskjema.
class Booking
{
    public Guid BookingId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string CustomerPhone { get; set; }
    public required Hotel Hotel { get; set; }
    public required Room Room { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

// Interface for alle bookingkontrakter.
interface IBookingContract
{
    Booking CreateBooking(Hotel hotel);
}

// Booking-skjemaet fungerer som en kontrakt fra kunden.
class BookingForm : IBookingContract, IValidatableBooking
{
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string CustomerPhone { get; set; }
    public RoomType RoomType { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public bool Validate(out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(CustomerName))
            errors.Add("Kundenavn må fylles ut.");

        if (string.IsNullOrWhiteSpace(CustomerEmail) || !CustomerEmail.Contains("@"))
            errors.Add("E-post må være gyldig.");

        if (string.IsNullOrWhiteSpace(CustomerPhone))
            errors.Add("Telefonnummer må fylles ut.");

        if (BookingDate.Date > CheckInDate.Date)
            errors.Add("Bestillingsdato kan ikke være etter innsjekkingsdato.");

        if (CheckInDate.Date >= CheckOutDate.Date)
            errors.Add("Utsjekkingsdato må være senere enn innsjekkingsdato.");

        if (BookingDate.Date < DateTime.Today)
            errors.Add("Bestillingsdato kan ikke være i fortiden.");

        return errors.Count == 0;
    }

    public Booking CreateBooking(Hotel hotel)
    {
        var room = hotel.AvailableRooms.FirstOrDefault(r => r.Type == RoomType && r.IsAvailable)
                   ?? hotel.AvailableRooms.FirstOrDefault(r => r.IsAvailable);

        var defaultRoom = new Room { Type = RoomType.Single, Price = 0, IsAvailable = false };
        return new Booking
        {
            CustomerName = CustomerName,
            CustomerEmail = CustomerEmail,
            CustomerPhone = CustomerPhone,
            Hotel = hotel,
            Room = room ?? defaultRoom,
            BookingDate = BookingDate,
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate
        };
    }
}

// Repository for bookinger som lagres i minnet.
class BookingRepository
{
    private readonly List<Booking> bookings = new List<Booking>();

    public void Add(Booking booking) => bookings.Add(booking);

    public bool Remove(Guid bookingId)
    {
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null) return false;
        bookings.Remove(booking);
        return true;
    }

    public List<Booking> GetByCustomerName(string customerName)
    {
        return bookings.Where(b => b.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}

// Controller som videresender bookingdata til riktig tjeneste og repository.
class BookingController
{
    private readonly IBookingService service;
    private readonly BookingRepository repository;
    private readonly Hotel hotel;

    public BookingController(Hotel hotel, IBookingService service, BookingRepository repository)
    {
        this.hotel = hotel;
        this.service = service;
        this.repository = repository;
    }

    public void ProcessBooking(IBookingContract contract)
    {
        if (contract is IValidatableBooking validatable && !validatable.Validate(out var errors))
        {
            Console.WriteLine("Validering feilet:");
            foreach (var error in errors)
            {
                Console.WriteLine($"- {error}");
            }
            return;
        }

        var booking = contract.CreateBooking(hotel);
        if (booking.Room == null)
        {
            Console.WriteLine("Ingen ledige rom for valgt type.");
            return;
        }

        service.BookRoom(booking);
        if (booking.BookingId != Guid.Empty)
        {
            repository.Add(booking);
        }
    }

    public void CancelBooking(Guid bookingId)
    {
        service.CancelBooking(bookingId);
        repository.Remove(bookingId);
    }

    public List<Booking> GetCustomerBookings(string customerName)
    {
        return repository.GetByCustomerName(customerName);
    }
}

// Servicen som gjennomfører bookingen.
class BookingService : IBookingService
{
    private List<Booking> bookings = new List<Booking>();

    public void BookRoom(Booking booking)
    {
        if (booking.Room.IsAvailable)
        {
            booking.BookingId = Guid.NewGuid();
            bookings.Add(booking);
            booking.Room.IsAvailable = false;
            Console.WriteLine($"Booking gjennomført! Booking-ID: {booking.BookingId}");
        }
        else
        {
            Console.WriteLine("Rommet er dessverre ikke tilgjengelig.");
        }
    }

    public void CancelBooking(Guid bookingId)
    {
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking != null)
        {
            booking.Room.IsAvailable = true;
            bookings.Remove(booking);
            Console.WriteLine($"Booking med ID {bookingId} er kansellert.");
        }
        else
        {
            Console.WriteLine("Fant ingen booking med den oppgitte ID-en.");
        }
    }

    public List<Booking> GetBookingsForCustomer(string customerName)
    {
        return bookings.Where(b => b.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}

class Program
{
    static void Main()
    {
        // Opprett et hotell med noen tilgjengelige rom.
        var hotel = new Hotel
        {
            Name = "Grand Hotel",
            Location = "Oslo",
            AvailableRooms = new List<Room>
            {
                new Room { Type = RoomType.Single, Price = 100, IsAvailable = true },
                new Room { Type = RoomType.Double, Price = 150, IsAvailable = true },
                new Room { Type = RoomType.Suite, Price = 300, IsAvailable = true }
            }
        };

        // initialiser service, repository og controller
        var bookingService = new BookingService();
        var bookingRepository = new BookingRepository();
        var controller = new BookingController(hotel, bookingService, bookingRepository);

        // Eksempel på booking som kan komme fra kundeskjema.
        var bookingForm = new BookingForm
        {
            CustomerName = "Ola Nordmann",
            CustomerEmail = "ola.nordmann@eksempel.com",
            CustomerPhone = "88888888",
            RoomType = RoomType.Single,
            BookingDate = DateTime.Now,
            CheckInDate = DateTime.Now.AddDays(5),
            CheckOutDate = DateTime.Now.AddDays(6)
        };

        // Gjør booking ut fra skjemaet
        controller.ProcessBooking(bookingForm);
        var customerBookings = controller.GetCustomerBookings(bookingForm.CustomerName);

        // Skriv ut antall bookinger for kunden
        Console.WriteLine($"Antall bookinger for {bookingForm.CustomerName}: {customerBookings.Count}");

        // Eksempel på kansellering av en booking hvis det finnes noen    
        if (customerBookings.Any())
        {
            // Kanseller den første bookingen for kunden basert på booking-ID
            controller.CancelBooking(customerBookings.First().BookingId);
            customerBookings = controller.GetCustomerBookings(bookingForm.CustomerName);

            // Skriv ut antall bookinger for kunden etter kansellering
            Console.WriteLine($"Antall bookinger for {bookingForm.CustomerName} etter kansellering: {customerBookings.Count}");
        }
    }
}
