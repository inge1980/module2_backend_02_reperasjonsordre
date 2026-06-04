using System.Collections.Generic;

namespace Core.Models;

// Representerer et hotell med en liste over rom.
public class Hotel
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required List<Room> AvailableRooms { get; set; }
}