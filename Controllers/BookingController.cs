using System;
using System.Collections.Generic;
using System.Linq;
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
        if (booking.Room == null)
        {
            Console.WriteLine("Ingen ledige rom for valgt type.");
            return;
        }

        service.BookRoom(booking);
        if (booking.BookingId != Guid.Empty)
        {
            repository.Add(booking);
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