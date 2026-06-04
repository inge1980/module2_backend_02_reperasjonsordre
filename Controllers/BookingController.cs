using Core.Models;
using Core.Contracts;
using Core.Repositories;

namespace Core.Controllers;

// Controller som videresender bookingdata til riktig tjeneste og repository.
public class BookingController
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

        var result = service.BookRoom(booking)
            .Bind(booked =>
            {
                repository.Save(booked);
                return new Success<Booking>(booked);
            });

        switch (result)
        {
            case Success<Booking> success:
                Console.WriteLine($"Booking registrert og lagret. Booking-ID: {success.Value.BookingId}");
                break;
            case Error<Booking> error:
                Console.WriteLine(error.Message);
                break;
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