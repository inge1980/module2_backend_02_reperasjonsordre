using Core.Models;

namespace Core.Contracts;

// Kontrakt for booking-tjenesten.
public interface IBookingService
{
    void BookRoom(Booking booking);
    void CancelBooking(Guid bookingId);
    List<Booking> GetBookingsForCustomer(string customerName);
}