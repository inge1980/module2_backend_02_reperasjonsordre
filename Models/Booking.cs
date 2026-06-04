using System;

namespace Core.Models;

// Modell for en booking som er opprettet fra et kundeskjema.
public class Booking
{
    public Guid BookingId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string CustomerPhone { get; set; }
    public required Hotel Hotel { get; set; }
    public required Room Room { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}