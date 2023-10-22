namespace Services.exceptions;

public class PassengerNotFoundException : Exception
{
    public PassengerNotFoundException(string flightId)
        : base($"No flight found with ID {flightId}")
    {
    }
}