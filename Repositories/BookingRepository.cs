using System;
using System.Collections.Generic;
using System.Linq;

// Repository for bookinger lagret i minnet.
// I en ekte applikasjon ville dette vært et lag som snakker med en database.
public class BookingRepository
{
    private readonly List<Booking> bookings = new List<Booking>();

    // Legg til en booking i repo
    public void Add(Booking booking) => bookings.Add(booking);

    // Fjern booking basert på bookingId
    public bool Remove(Guid bookingId)
    {
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null) return false;
        bookings.Remove(booking);
        return true;
    }

    // Hent alle bookinger for en kunde
    public List<Booking> GetByCustomerName(string customerName)
    {
        return bookings.Where(b => b.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}