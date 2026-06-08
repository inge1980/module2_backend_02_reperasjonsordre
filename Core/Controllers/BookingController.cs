using Core.Models;
using Core.Contracts;
using Core.Repositories;

namespace Core.Controllers;

// Controller som videresender bookingdata til riktig tjeneste og repository.
public class BookingController
{
    private readonly IBookingService service;
    private readonly IBookingRepository repository;
    private readonly Hotel hotel;

    public BookingController(Hotel hotel, IBookingService service, IBookingRepository repository)
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

        Console.Clear();
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
        var cancelResult = service.CancelBooking(bookingId)
            .Bind(cancelled => repository.Remove(bookingId));

        switch (cancelResult)
        {
            case Success<bool>:
                Console.WriteLine($"Booking med ID {bookingId} er kansellert.");
                break;
            case Error<bool> error:
                Console.WriteLine($"Kansellering feilet: {error.Message}");
                break;
        }
    }

    public List<Booking> GetCustomerBookings(string customerName)
    {
        return repository.GetByCustomerName(customerName);
    }
}