// Kontrakt for booking-konstruksjon fra et skjema eller lignende.
public interface IBookingContract
{
    Booking CreateBooking(Hotel hotel);
}