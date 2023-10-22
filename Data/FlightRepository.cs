using System.Globalization;
using System.IO.Abstractions;
using CsvHelper;
using CsvHelper.Configuration;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models;

namespace Data;

public class FlightRepository : IFlightRepository
{
    private readonly string _filePath;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<IFlightRepository> _logger;

    public FlightRepository(string filePath, IFileSystem fileSystem, ILogger<IFlightRepository> logger)
    {
        _filePath = filePath;
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public List<Flight> GetAllFlights()
    {
        try
        {
            if (!_fileSystem.File.Exists(_filePath))
                return new List<Flight>();

            using (var fileStream = _fileSystem.File.OpenRead(_filePath))
            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var flights = csv.GetRecords<Flight>().ToList();
                return flights;
            }
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "File not found.");
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access to the file.");
            throw;
        }
        catch (CsvHelperException ex)
        {
            _logger.LogError(ex, "CSV processing error.");
            throw;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "IO operation error.");
            throw;
        }
    }

    public void SaveFlights(List<Flight> flights)
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using var stream = _fileSystem.File.Open(_filePath, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);

            
            if (stream.Length == 0)
            {
                csv.WriteHeader<Flight>();
                csv.NextRecord();
            }

            csv.WriteRecords(flights);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access to the file.");
            throw;
        }
        catch (CsvHelperException ex)
        {
            _logger.LogError(ex, "CSV processing error.");
            throw;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "IO operation error.");
            throw;
        }
    }

    public List<Flight> GetFlights(FlightSearchParams flightSearchParams)
    {
        var flights = GetAllFlights();
        return flights.Where(flight =>
            (string.IsNullOrEmpty(flightSearchParams.Id) || flight.Id == flightSearchParams.Id) &&
            (string.IsNullOrEmpty(flightSearchParams.DepartureCountry) ||
             flight.DepartureCountry == flightSearchParams.DepartureCountry) &&
            (string.IsNullOrEmpty(flightSearchParams.DestinationCountry) ||
             flight.DestinationCountry == flightSearchParams.DestinationCountry) &&
            (flightSearchParams.DepartureDate == null || flight.DepartureDate >= flightSearchParams.DepartureDate) &&
            (string.IsNullOrEmpty(flightSearchParams.DepartureAirport) ||
             flight.DepartureAirport == flightSearchParams.DepartureAirport) &&
            (string.IsNullOrEmpty(flightSearchParams.ArrivalAirport) ||
             flight.ArrivalAirport == flightSearchParams.ArrivalAirport) &&
            (flightSearchParams.ClassType.ToString().Equals("0") || CheckAvailabilityAndPrice(flight,
                flightSearchParams.ClassType.ToString(), flightSearchParams.MaxPrice))
        ).ToList();
    }

    public void UpdateFlight(Flight updatedFlight)
    {
        var flights = GetAllFlights();

        for (var i = 0; i < flights.Count; i++)
            if (flights[i].Id == updatedFlight.Id)
            {
                flights[i] = updatedFlight;
                break;
            }

        SaveAllFlights(flights); 
    }

    public void DeleteFlight(string flightId)
    {
        var flights = GetAllFlights();

        flights.RemoveAll(f => f.Id == flightId);

        SaveAllFlights(flights);

    }

    public ImportResult ImportFlightsFromCSV(string filePath)
    {
        var result = new ImportResult();

        if (!_fileSystem.File.Exists(_filePath))
        {
            result.Status = ImportStatus.Failed;
            result.Errors.Add($"Error: No such file {filePath}");
            return result;
        }
        
        var allFlights = GetAllFlights();

        try
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var flights = csv.GetRecords<Flight>().ToList();

                foreach (var flight in flights)
                {
                    if (allFlights.Contains(flight))
                    {
                        continue;
                    }
                    
                    bool valid = true;
                    if (string.IsNullOrEmpty(flight.Id))
                    {
                        result.Errors.Add("Flight has no ID.");
                        valid = false;
                    }

                    if (string.IsNullOrEmpty(flight.DepartureCountry))
                    {
                        result.Errors.Add($"Flight {flight.Id} has no DepartureCountry.");
                        valid = false;
                    }

                    if (string.IsNullOrEmpty(flight.ArrivalAirport))
                    {
                        result.Errors.Add($"Flight {flight.Id} has no ArrivalCountry.");
                        valid = false;
                    }

                    if (flight.DepartureDate <= DateTime.Now)
                    {
                        result.Errors.Add($"Flight {flight.Id} has a DepartureDate in the past.");
                        valid = false;
                    }

                    if (valid)
                    {
                        try
                        {
                            SaveFlights(new List<Flight>{flight});
                        }
                        catch (Exception saveEx)
                        {
                            result.Errors.Add($"Error saving flight {flight.Id}: {saveEx.Message}");
                        }
                    }
                    else
                    {
                        result.Status = ImportStatus.PartiallySuccessful;
                        result.Errors.Add($"Error saving flight {flight.Id}: Fix errors");
                    }
                   
                }
            }
        }
        catch (Exception ex)
        {
            result.Status = ImportStatus.InvalidFileFormat;
            result.Errors.Add($"Error processing file: {ex.Message}");
        }

        if (!result.Errors.Any())
            result.Status = ImportStatus.Success;
        else
            result.Status = ImportStatus.Failed;

        return result;
    }

    private bool CheckAvailabilityAndPrice(Flight flight, string flightClass, decimal? maxPrice)
    {
        var isAvailable = false;
        var isWithinPrice = false;

        switch (flightClass)
        {
            case "ECONOMY":
                isAvailable = flight.AvailableSeatsEconomy > 0;
                isWithinPrice = maxPrice == null || flight.PriceEconomy <= maxPrice;
                break;
            case "BUSINESS":
                isAvailable = flight.AvailableSeatsBusiness > 0;
                isWithinPrice = maxPrice == null || flight.PriceBusiness <= maxPrice;
                break;
            case "FIRST_CLASS":
                isAvailable = flight.AvailableSeatsFirstClass > 0;
                isWithinPrice = maxPrice == null || flight.PriceFirstClass <= maxPrice;
                break;
        }

        return isAvailable && isWithinPrice;
    }

    private void SaveAllFlights(List<Flight> flights)
    {
        try
        {
            using var stream = _fileSystem.File.OpenWrite(_filePath);
            using var writer = new StreamWriter(stream);
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                 csv.WriteRecords(flights);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access to the file.");
            throw;
        }
        catch (CsvHelperException ex)
        {
            _logger.LogError(ex, "CSV processing error.");
            throw;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "IO operation error.");
            throw;
        }
    }
}