namespace Models;

public class FlightSearchParams
{
    public string Id { get; set; }
    public string DepartureCountry { get; set; }
    public string DestinationCountry { get; set; }
    public DateTime? DepartureDate { get; set; }
    public string DepartureAirport { get; set; }
    public string ArrivalAirport { get; set; }
    public decimal? MaxPrice { get; set; }
    public ClassType ClassType { get; set; }
}