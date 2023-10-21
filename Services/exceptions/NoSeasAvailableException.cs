namespace Services.exceptions;

public class NoSeasAvailableException : Exception
{
    public NoSeasAvailableException(string flightId)
        : base($"No Seats available for the flight {flightId}")
    {
    }
}