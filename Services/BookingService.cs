using Core.Models;
using Core.Contracts;

namespace Core.Services;

// Servicen som håndterer booking-logikk.
public class BookingService : IBookingService
{
    private List<Booking> bookings = new List<Booking>();

    public Result<Booking> BookRoom(Booking booking)
    {
        if (booking.Room.IsAvailable)
        {
            booking.BookingId = Guid.NewGuid();
            bookings.Add(booking);
            booking.Room.IsAvailable = false;
            return new Success<Booking>(booking);
        }

        return new Error<Booking>("Rommet er dessverre ikke tilgjengelig.");
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