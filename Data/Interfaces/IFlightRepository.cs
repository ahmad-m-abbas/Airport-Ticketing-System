using Models;

namespace Data.Interfaces;

public interface IFlightRepository
{
    List<Flight> GetAllFlights();
    void SaveFlights(List<Flight> flights);
    List<Flight> GetFlights(FlightSearchParams flightSearchParams);
    void UpdateFlight(Flight updatedFlight);
    void DeleteFlight(string flightId);
    ImportResult ImportFlightsFromCSV(string filePath);
}