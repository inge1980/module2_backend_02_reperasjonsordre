using System;
using System.Linq;
using System.Collections.Generic;
using Core.Contracts;

namespace Core.Models;

// Booking-skjemaet fungerer som en kontrakt fra kunden og inneholder validering.
public class BookingForm : IBookingContract, IValidatableBooking
{
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string CustomerPhone { get; set; }
    public RoomType RoomType { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public bool Validate(out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(CustomerName))
            errors.Add("Kundenavn må fylles ut.");

        if (string.IsNullOrWhiteSpace(CustomerEmail) || !CustomerEmail.Contains("@"))
            errors.Add("E-post må være gyldig.");

        if (string.IsNullOrWhiteSpace(CustomerPhone))
            errors.Add("Telefonnummer må fylles ut.");

        if (BookingDate.Date > CheckInDate.Date)
            errors.Add("Bestillingsdato kan ikke være etter innsjekkingsdato.");

        if (CheckInDate.Date >= CheckOutDate.Date)
            errors.Add("Utsjekkingsdato må være senere enn innsjekkingsdato.");

        if (BookingDate.Date < DateTime.Today)
            errors.Add("Bestillingsdato kan ikke være i fortiden.");

        return errors.Count == 0;
    }

    public Booking CreateBooking(Hotel hotel)
    {
        var room = hotel.AvailableRooms.FirstOrDefault(r => r.Type == RoomType && r.IsAvailable)
                   ?? hotel.AvailableRooms.FirstOrDefault(r => r.IsAvailable);

        var defaultRoom = new Room { Type = RoomType.Single, Price = 0, IsAvailable = false };
        return new Booking
        {
            CustomerName = CustomerName,
            CustomerEmail = CustomerEmail,
            CustomerPhone = CustomerPhone,
            Hotel = hotel,
            Room = room ?? defaultRoom,
            BookingDate = BookingDate,
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate
        };
    }
}