using System;

// Romtyper og representasjon av et rom ved hotellet.
public enum RoomType
{
    Single,
    Double,
    Suite
}

public class Room
{
    public RoomType Type { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}