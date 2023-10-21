using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using Data;
using Models;

namespace Airport_Ticket_Tests;

[TestFixture]
public class FlightRepositoryUnitTests
{
    [SetUp]
    public void Setup()
    {
        MockFileSystem = new MockFileSystem();
        MockFileSystem.AddFile(FlightFileTestPath, new MockFileData(""));
        _repository = new FlightRepository(FlightFileTestPath, MockFileSystem, null);
    }

    protected MockFileSystem MockFileSystem;
    protected readonly string FlightFileTestPath = "/flights.csv";
    private FlightRepository _repository;
    private readonly Fixture _fixture;

    public FlightRepositoryUnitTests()
    {
        _fixture = new Fixture();
    }


    [Test]
    public void GetAllFlights_ReturnsExpectedFlights()
    {
        var sampleFlights = _fixture.CreateMany<Flight>(5).ToList();

        _repository.SaveFlights(sampleFlights);

        var flights = _fixture.CreateMany<Flight>(5).ToList();

        Assert.AreEqual(5, flights.Count);
    }

    [Test]
    public void SaveFlights_SavesCorrectNumberOfFlights()
    {
        var flights = _fixture.CreateMany<Flight>(5).ToList();
        _repository.SaveFlights(flights);

        var fileContents = MockFileSystem.File.ReadAllText(FlightFileTestPath);
        var lines = fileContents.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(flights.Count + 1, lines.Length); // +1 for the header
    }

    [Test]
    public void GetFlights_ReturnsExpectedFlightsBasedOnSearchParams()
    {
        var flights = new List<Flight>
        {
            new() { Id = "1", DepartureCountry = "USA", ArrivalAirport = "Amman" },
            new() { Id = "2", DepartureCountry = "Amman", ArrivalAirport = "Frankfurt" },
            new() { Id = "3", DepartureCountry = "Bosnia", ArrivalAirport = "Amman" },
            new() { Id = "4", DepartureCountry = "Istanbul", ArrivalAirport = "Amman" }
        };

        _repository.SaveFlights(flights);

        var searchParams = new FlightSearchParams
        {
            DepartureCountry = "USA",
            ArrivalAirport = "Amman"
        };

        var results = _repository.GetFlights(searchParams);

        Assert.AreEqual(1, results.Count);
    }

    [Test]
    public void UpdateFlight_UpdatesFlightDetailsCorrectly()
    {
        var flights = new List<Flight>
        {
            new() { Id = "1", DepartureCountry = "USA" },
            new() { Id = "2", DepartureCountry = "Amman" }
        };
        _repository.SaveFlights(flights);

        var updatedFlight = new Flight
        {
            Id = "1", ArrivalAirport = "Amman"
        };

        _repository.UpdateFlight(updatedFlight);

        var allFlights = _repository.GetAllFlights();
        var retrievedFlight = allFlights.First(f => f.Id == updatedFlight.Id);

        Assert.AreEqual(updatedFlight.ArrivalAirport, retrievedFlight.ArrivalAirport);
    }

    [Test]
    public void DeleteFlight_RemovesFlightFromRepository()
    {
        var flights = new List<Flight>
        {
            new() { Id = "1", DepartureCountry = "USA" },
            new() { Id = "2", DepartureCountry = "Amman" }
        };
        _repository.SaveFlights(flights);

        var flightToDeleteId = "1";
        _repository.DeleteFlight(flightToDeleteId);

        var allFlights = _repository.GetAllFlights();

        Assert.IsFalse(allFlights.Any(f => f.Id == flightToDeleteId));
    }
}