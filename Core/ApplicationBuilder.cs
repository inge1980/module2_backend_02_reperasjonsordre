using Core.Models;
using Core.Services;
using Core.Repositories;
using Core.Controllers;
using Core.Contracts;

namespace Core;

// Builder for å opprette og konfigurere applikasjonen.
public static class ApplicationBuilder
{
    public static BookingController CreateBookingController()
    {
        var hotel = HotelFactory.CreateDefaultHotel();
        var bookingService = new BookingService();
        var bookingRepository = new BookingRepository();
        return new BookingController(hotel, bookingService, bookingRepository);
    }
}
