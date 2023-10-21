namespace Models;

public class BookingSearchParams
{
    public string Id { get; set; }
    public string FlightId { get; set; }
    public ClassType? ClassType { get; set; }
    public string PassengerId { get; set; }
    public Status? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string PassengerName { get; set; }
}