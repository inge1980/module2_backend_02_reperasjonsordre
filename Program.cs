// En enkel hotellbookingmodell som viser hvordan nettbasert skjema-data kan
// transformeres til en bookingkontrakt og behandles gjennom service og repository.

// Interfaces er flyttet til `Contracts/`.
// Se: Contracts/IBookingService.cs, Contracts/IBookingContract.cs, Contracts/IValidatableBooking.cs


// Interface for alle bookingkontrakter er flyttet til `Contracts/IBookingContract.cs`.

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
