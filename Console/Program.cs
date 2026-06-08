using Core.Models;
using Core.Contracts;
using Core;

// En enkel hotellbookingmodell som viser hvordan nettbasert skjema-data kan
// transformeres til en bookingkontrakt og behandles gjennom service og repository.

class Program
{
    static void Main()
    {
        // Opprett controller med alle dependencies
        var controller = ApplicationBuilder.CreateBookingController();

        Console.WriteLine("Interaktiv test for booking.");
        while (true)
        {
            Console.Write("Kundenavn: ");
            var name = Console.ReadLine();

            Console.Write("E-post (default: navn@eksempel.com): ");
            var email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email)) email = "navn@eksempel.com";

            Console.Write("Telefon (default: 88888888): ");
            var phone = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phone)) phone = "88888888";

            Console.Write("Romtype (Single/Double/Suite) (default: Single): ");
            var roomTypeInput = Console.ReadLine();
            var roomType = RoomType.Single;
            if (!string.IsNullOrWhiteSpace(roomTypeInput) && Enum.TryParse<RoomType>(roomTypeInput, true, out var parsedRoomType))
                roomType = parsedRoomType;

            var now = DateTime.Now;
            Console.Write($"Bestillingsdato (yyyy-MM-dd) (default: {now:yyyy-MM-dd}): ");
            var bookingDateInput = Console.ReadLine();
            var bookingDate = DateTime.TryParse(bookingDateInput, out var parsedBookingDate) ? parsedBookingDate : now;

            Console.Write($"Innsjekk (yyyy-MM-dd) (default: {now.AddDays(5):yyyy-MM-dd}): ");
            var checkInInput = Console.ReadLine();
            var checkIn = DateTime.TryParse(checkInInput, out var parsedCheckIn) ? parsedCheckIn : now.AddDays(5);

            Console.Write($"Utsjekk (yyyy-MM-dd) (default: {now.AddDays(6):yyyy-MM-dd}): ");
            var checkOutInput = Console.ReadLine();
            var checkOut = DateTime.TryParse(checkOutInput, out var parsedCheckOut) ? parsedCheckOut : now.AddDays(6);

            var bookingForm = new BookingForm
            {
                CustomerName = name ?? "",
                CustomerEmail = email,
                CustomerPhone = phone,
                RoomType = roomType,
                BookingDate = bookingDate,
                CheckInDate = checkIn,
                CheckOutDate = checkOut
            };

            if (bookingForm is IValidatableBooking valid && !valid.Validate(out var errors))
            {
                Console.WriteLine("Validering feilet:");
                foreach (var err in errors) Console.WriteLine($"- {err}");
                continue;
            }

            controller.ProcessBooking(bookingForm);

            var customerBookings = controller.GetCustomerBookings(bookingForm.CustomerName);
            if (customerBookings.Count == 0)
            {
                Console.WriteLine($"Ingen bookinger for {bookingForm.CustomerName}.");
            }
            else
            {
                Console.WriteLine($"Bookinger for {bookingForm.CustomerName}:");
                foreach (var b in customerBookings)
                {
                    Console.WriteLine($"- BookingId: {b.BookingId}");
                    Console.WriteLine($"  Romtype: {b.Room.Type}, Pris: {b.Room.Price}");
                    Console.WriteLine($"  BookingDate: {b.BookingDate:yyyy-MM-dd}, CheckIn: {b.CheckInDate:yyyy-MM-dd}, CheckOut: {b.CheckOutDate:yyyy-MM-dd}");
                    Console.WriteLine($"  E-post: {b.CustomerEmail}, Telefon: {b.CustomerPhone}");
                }

                Console.Write("Kanseller første booking? (j/N): ");
                var cancel = Console.ReadLine();
                if (string.Equals(cancel, "j", StringComparison.OrdinalIgnoreCase))
                {
                    controller.CancelBooking(customerBookings.First().BookingId);
                    customerBookings = controller.GetCustomerBookings(bookingForm.CustomerName);
                    Console.WriteLine($"Bookinger etter kansellering: {customerBookings.Count}");
                }
            }

            Console.WriteLine("Vil du teste en ny booking? (j/N): ");
            var again = Console.ReadLine();
            if (!string.Equals(again, "j", StringComparison.OrdinalIgnoreCase)) break;
        }
    }

}
