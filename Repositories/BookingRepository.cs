using Core.Models;

namespace Core.Repositories;

// Repository for bookinger lagret i minnet.
// I en ekte applikasjon ville dette vært et lag som snakker med en database.
public class BookingRepository
{
    private readonly List<Booking> _bookings = new();

    // Lagre en booking i repo
    public void Save(Booking booking) => _bookings.Add(booking);

    // Fjern booking basert på bookingId
    public bool Remove(Guid bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null) return false;
        _bookings.Remove(booking);
        return true;
    }

    // Hent alle bookinger for en kunde
    public List<Booking> GetByCustomerName(string customerName)
    {
        return _bookings.Where(b => b.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}