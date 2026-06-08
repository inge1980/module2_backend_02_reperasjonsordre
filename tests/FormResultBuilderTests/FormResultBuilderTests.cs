using Core.Models;
namespace Tests.FormResultBuilderTests;

public class FormResultBuilderTests
{
    [Fact]
    public void BookingForm_WithValidData_ValidateReturnsTrue()
    {
        var form = new BookingForm
        {
            CustomerName = "John",
            CustomerEmail = "john@example.com",
            CustomerPhone = "12345678",
            RoomType = RoomType.Double,
            BookingDate = DateTime.Today,
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(2)
        };

        var isValid = form.Validate(out var errors);

        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void BookingForm_BindValidRoomType_WithValidRoomType_ReturnsSuccess()
    {
        var form = new BookingForm
        {
            CustomerName = "John",
            CustomerEmail = "john@example.com",
            CustomerPhone = "12345678",
            BookingDate = DateTime.Today,
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(2)
        };

        var result = new Success<BookingForm>(form)
            .BindValidRoomType("Double");

        var success = Assert.IsType<Success<BookingForm>>(result);
        Assert.Equal(RoomType.Double, success.Value.RoomType);
    }
}