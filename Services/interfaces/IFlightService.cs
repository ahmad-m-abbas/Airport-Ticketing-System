using Models;

namespace Services.interfaces;

public interface IFlightService
{
    List<Flight> SearchFlights(FlightSearchParams flightSearchParams);
    Flight GetFlightById(string id);
    void Update(Flight flight);
    Dictionary<string, List<string>> GetFlightDataConstraints();
    ImportResult ImportFlightsFromCSV(string filePath);
}