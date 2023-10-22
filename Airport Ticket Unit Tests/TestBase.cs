using System.IO.Abstractions.TestingHelpers;

namespace Airport_Ticket_Tests;

public class TestBase : IDisposable
{
    protected readonly string BookingsFileTestPath = "bookings.csv";
    protected readonly string FlightFileTestPath = "flights.csv";
    protected readonly string PassengersFileTestPath = "passengers.csv";
    protected MockFileSystem MockFileSystem;

    public TestBase()
    {
        MockFileSystem = new MockFileSystem();
        MockFileSystem.AddFile(FlightFileTestPath, new MockFileData(""));
        MockFileSystem.AddFile(BookingsFileTestPath, new MockFileData(""));
        MockFileSystem.AddFile(PassengersFileTestPath, new MockFileData(""));
    }

    public void Dispose()
    {
    }
}