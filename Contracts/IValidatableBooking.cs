using System.Collections.Generic;

namespace Core.Contracts;

// Kontrakt for booking-skjemaer som kan valideres før behandling.
public interface IValidatableBooking
{
    bool Validate(out List<string> errors);
}