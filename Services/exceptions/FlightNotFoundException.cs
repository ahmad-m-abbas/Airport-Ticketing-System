namespace Services.exceptions;

public class FlightNotFoundException : Exception
{
    public FlightNotFoundException(string flightId)
        : base($"No flight found with ID {flightId}")
    {
    }
}