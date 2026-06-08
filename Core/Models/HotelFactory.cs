namespace Core.Models;

// Factory for å opprette og konfigurere hoteller.
public static class HotelFactory
{
    public static Hotel CreateDefaultHotel()
    {
        return new Hotel
        {
            Name = "Grand Hotel",
            Location = "Oslo",
            AvailableRooms = [
                new Room { Type = RoomType.Single, Price = 100, IsAvailable = true },
                new Room { Type = RoomType.Double, Price = 150, IsAvailable = true },
                new Room { Type = RoomType.Suite, Price = 300, IsAvailable = true }
            ]
        };
    }
}
