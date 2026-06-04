using Core.Models;
using Core.Services;
using Core.Repositories;
using Core.Contracts;
using System.Linq;
using Core.Controllers;
using Core;

// En enkel hotellbookingmodell som viser hvordan nettbasert skjema-data kan
// transformeres til en bookingkontrakt og behandles gjennom service og repository.

class Program
{
    static void Main()
    {
        // Opprett controller med alle dependencies
        var controller = ApplicationBuilder.CreateBookingController();

        Console.WriteLine("Interaktiv test for booking. Trykk Enter uten navn for å avslutte.");
        while (true)
        {
            Console.Write("Kundenavn: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) break;

            Console.Write("E-post (default: navn@eksempel.com): ");
            var email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email)) email = name.Replace(" ", ".").ToLower() + "@eksempel.com";

            Console.Write("Telefon (default: 88888888): ");
            var phone = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phone)) phone = "88888888";

            Console.Write("Romtype (Single/Double/Suite) (default: Single): ");
            var roomTypeInput = Console.ReadLine();
            var roomType = RoomType.Single;
            if (!string.IsNullOrWhiteSpace(roomTypeInput) && Enum.TryParse<RoomType>(roomTypeInput, true, out var rt))
                roomType = rt;

            var now = DateTime.Now;
            Console.Write($"Bestillingsdato (yyyy-MM-dd) (default: {now:yyyy-MM-dd}): ");
            var bookingDateInput = Console.ReadLine();
            var bookingDate = DateTime.TryParse(bookingDateInput, out var bd) ? bd : now;

            Console.Write($"Innsjekk (yyyy-MM-dd) (default: {now.AddDays(5):yyyy-MM-dd}): ");
            var checkInInput = Console.ReadLine();
            var checkIn = DateTime.TryParse(checkInInput, out var ci) ? ci : now.AddDays(5);

            Console.Write($"Utsjekk (yyyy-MM-dd) (default: {now.AddDays(6):yyyy-MM-dd}): ");
            var checkOutInput = Console.ReadLine();
            var checkOut = DateTime.TryParse(checkOutInput, out var co) ? co : now.AddDays(6);

            var bookingForm = new BookingForm
            {
                CustomerName = name,
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
            Console.WriteLine($"Antall bookinger for {bookingForm.CustomerName}: {customerBookings.Count}");

            if (customerBookings.Any())
            {
                Console.Write("Kanseller første booking? (j/N): ");
                var cancel = Console.ReadLine();
                if (string.Equals(cancel, "j", StringComparison.OrdinalIgnoreCase))
                {
                    controller.CancelBooking(customerBookings.First().BookingId);
                    customerBookings = controller.GetCustomerBookings(bookingForm.CustomerName);
                    Console.WriteLine($"Antall bookinger for {bookingForm.CustomerName} etter kansellering: {customerBookings.Count}");
                }
            }

            Console.WriteLine("Vil du teste en ny booking? (j/N): ");
            var again = Console.ReadLine();
            if (!string.Equals(again, "j", StringComparison.OrdinalIgnoreCase)) break;
        }
    }

    static Result<int> ParseInt(string input)
    {
        return int.TryParse(input, out var value)
            ? new Success<int>(value)
            : new Error<int>($"'{input}' er ikke et gyldig tall.");
    }

    static Result<decimal> Divide(int value, int divisor)
    {
        return divisor == 0
            ? new Error<decimal>("Kan ikke dele på null.")
            : new Success<decimal>(value / (decimal)divisor);
    }
}
