using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using Data;
using Models;

namespace Airport_Ticket_Tests;

[TestFixture]
public class BookingRepositoryUnitTests
{
    [SetUp]
    public void Setup()
    {
        MockFileSystem = new MockFileSystem();
        MockFileSystem.AddFile(BookingFileTestPath, new MockFileData(""));
        _repository = new BookingRepository(BookingFileTestPath, MockFileSystem, null);
    }

    protected MockFileSystem MockFileSystem;
    protected readonly string BookingFileTestPath = "/bookings.csv";
    private BookingRepository _repository;
    private readonly Fixture _fixture;

    public BookingRepositoryUnitTests()
    {
        _fixture = new Fixture();
    }


    [Test]
    public void GetAllBookings_ReturnsExpectedBookings()
    {
        var sampleBookings = _fixture.CreateMany<Booking>(5).ToList();

        _repository.SaveBookings(sampleBookings);

        var bookings = _repository.GetAllBookings();

        Assert.AreEqual(5, bookings.Count);
    }

    [Test]
    public void SaveFlights_SavesCorrectNumberOfFlights()
    {
        var sampleBookings = _fixture.CreateMany<Booking>(5).ToList();

        _repository.SaveBookings(sampleBookings);

        var fileContents = MockFileSystem.File.ReadAllText(BookingFileTestPath);
        var lines = fileContents.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(sampleBookings.Count + 1, lines.Length); // +1 for the header
    }

    [Test]
    public void GetFlights_ReturnsExpectedFlightsBasedOnSearchParams()
    {
        var bookings = new List<Booking>
        {
            new()
            {
                Id = "1", FlightId = "1", ClassType = ClassType.ECONOMY, PassengerId = "1", Status = Status.BOOKED
            },
            new()
            {
                Id = "2", FlightId = "1", ClassType = ClassType.BUSINESS, PassengerId = "3", Status = Status.BOOKED
            },
            new()
            {
                Id = "3", FlightId = "2", ClassType = ClassType.ECONOMY, PassengerId = "4", Status = Status.CANCELLED
            },
            new()
            {
                Id = "4", FlightId = "2", ClassType = ClassType.FIRST_CLASS, PassengerId = "1",
                Status = Status.CANCELLED
            },
            new() { Id = "5", FlightId = "2", ClassType = ClassType.ECONOMY, PassengerId = "2", Status = Status.BOOKED }
        };

        _repository.SaveBookings(bookings);

        var searchParams = new BookingSearchParams
        {
            PassengerId = "1"
        };

        var results = _repository.GetBookings(searchParams);

        Assert.AreEqual(2, results.Count);
    }

    [Test]
    public void UpdateFlight_UpdatesFlightDetailsCorrectly()
    {
        var bookings = new List<Booking>
        {
            new()
            {
                Id = "1", FlightId = "1", ClassType = ClassType.ECONOMY, PassengerId = "1", Status = Status.BOOKED
            },
            new()
            {
                Id = "2", FlightId = "1", ClassType = ClassType.BUSINESS, PassengerId = "3", Status = Status.BOOKED
            },
            new()
            {
                Id = "3", FlightId = "2", ClassType = ClassType.ECONOMY, PassengerId = "4", Status = Status.CANCELLED
            },
            new()
            {
                Id = "4", FlightId = "2", ClassType = ClassType.FIRST_CLASS, PassengerId = "1",
                Status = Status.CANCELLED
            },
            new() { Id = "5", FlightId = "2", ClassType = ClassType.ECONOMY, PassengerId = "2", Status = Status.BOOKED }
        };


        _repository.SaveBookings(bookings);

        var updatedBooking = new Booking
        {
            Id = "1", ClassType = ClassType.BUSINESS
        };

        _repository.Update(updatedBooking);

        var allBookings = _repository.GetAllBookings();
        var retrievedFlight = allBookings.First(b => b.Id == updatedBooking.Id);

        Assert.AreEqual(updatedBooking.ClassType, retrievedFlight.ClassType);
    }

    [Test]
    public void DeleteFlight_RemovesFlightFromRepository()
    {
        var bookings = new List<Booking>
        {
            new()
            {
                Id = "1", FlightId = "1", ClassType = ClassType.ECONOMY, PassengerId = "1", Status = Status.BOOKED
            },
            new()
            {
                Id = "2", FlightId = "1", ClassType = ClassType.BUSINESS, PassengerId = "3", Status = Status.BOOKED
            },
            new()
            {
                Id = "3", FlightId = "2", ClassType = ClassType.ECONOMY, PassengerId = "4", Status = Status.CANCELLED
            },
            new()
            {
                Id = "4", FlightId = "2", ClassType = ClassType.FIRST_CLASS, PassengerId = "1",
                Status = Status.CANCELLED
            },
            new() { Id = "5", FlightId = "2", ClassType = ClassType.ECONOMY, PassengerId = "2", Status = Status.BOOKED }
        };


        _repository.SaveBookings(bookings);

        var bookingToDeleteId = "1";
        _repository.Delete(bookingToDeleteId);

        var allFliBookings = _repository.GetAllBookings();

        Assert.IsFalse(allFliBookings.Any(b => b.Id == bookingToDeleteId));
    }
}