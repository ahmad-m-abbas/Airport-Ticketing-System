namespace Services.exceptions;

public class BookingAlreadyExists : Exception
{
    public BookingAlreadyExists(string passengerId, string flightId) : base(
        $"Booking Already exists for the passenger with Id {passengerId} and flight id: {flightId}")
    {
    }
}