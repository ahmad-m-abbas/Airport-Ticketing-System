using Data;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models;
using Services.exceptions;
using Services.interfaces;

namespace Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<FlightService> _logger;

    public FlightService(IFlightRepository flightRepository, ILogger<FlightService> logger)
    {
        _flightRepository = flightRepository;
        _logger = logger;
    }

    public List<Flight> SearchFlights(FlightSearchParams flightSearchParams)
    {
        return _flightRepository.GetFlights(flightSearchParams);
    }


    public Flight GetFlightById(string id)
    {
        var flightSearchParams = new FlightSearchParams { Id = id };
        var flights = _flightRepository.GetFlights(flightSearchParams);

        if (flights.Count == 0) throw new FlightNotFoundException(id);
        return flights[0];
    }

    public void Update(Flight flight)
    {
        _flightRepository.UpdateFlight(flight);
    }

    public Dictionary<string, List<string>> GetFlightDataConstraints()
    {
        var constraints = new Dictionary<string, List<string>>();
        var properties = typeof(Flight).GetProperties();

        foreach (var property in properties)
        {
            var validationAttributes = property.GetCustomAttributes(typeof(ValidationDescriptionAttribute), false);

            var validationDescriptions = validationAttributes
                .OfType<ValidationDescriptionAttribute>()
                .Select(a => a.Description)
                .ToList();

            if (validationDescriptions.Count > 0) constraints.Add(property.Name, validationDescriptions);
        }

        return constraints;
    }

    public ImportResult ImportFlightsFromCSV(string filePath)
    {
        return _flightRepository.ImportFlightsFromCSV(filePath);
    }
}