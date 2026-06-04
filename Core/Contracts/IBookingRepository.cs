using Core.Models;

namespace Core.Contracts;

// Kontrakt for booking-repositoriet.
public interface IBookingRepository
{
    void Save(Booking booking);
    Result<bool> Remove(Guid bookingId);
    List<Booking> GetByCustomerName(string customerName);
}
