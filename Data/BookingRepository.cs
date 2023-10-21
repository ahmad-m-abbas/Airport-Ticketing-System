using System.Globalization;
using System.IO.Abstractions;
using CsvHelper;
using CsvHelper.Configuration;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models;

namespace Data;

public class BookingRepository : IBookingRepository
{
    private readonly IFileSystem _fileSystem;
    private readonly string _filePath;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(string filePath, IFileSystem fileSystem, ILogger<BookingRepository> logger)
    {
        _filePath = filePath;
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public List<Booking> GetAllBookings()
    {
        try
        {
            if (!_fileSystem.File.Exists(_filePath))
                return new List<Booking>();

            using var fileStream = _fileSystem.File.OpenRead(_filePath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<Booking>().ToList();
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

    public void SaveBookings(List<Booking> bookings)
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using var stream = _fileSystem.File.Open(_filePath, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using (var csv = new CsvWriter(writer, config))
            {
                if (!_fileSystem.File.Exists(_filePath))
                {
                    csv.WriteHeader<Booking>();
                    csv.NextRecord();
                }

                csv.WriteRecords(bookings);
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

    public List<Booking> GetBookings(BookingSearchParams searchParams)
    {
        
        var allBookings = GetAllBookings();
        if (!string.IsNullOrEmpty(searchParams.Id))
            allBookings = allBookings.Where(b => b.Id == searchParams.Id).ToList();
        if (!string.IsNullOrEmpty(searchParams.FlightId))
            allBookings = allBookings.Where(b => b.FlightId == searchParams.FlightId).ToList();
        if (searchParams.ClassType.HasValue)
            allBookings = allBookings.Where(b => b.ClassType == searchParams.ClassType.Value).ToList();
        if (!string.IsNullOrEmpty(searchParams.PassengerId))
            allBookings = allBookings.Where(b => b.PassengerId == searchParams.PassengerId).ToList();
        if (searchParams.Status.HasValue)
            allBookings = allBookings.Where(b => b.Status == searchParams.Status.Value).ToList();
        
        return allBookings;
    }

    public void Update(Booking updatedBooking)
    {

        var bookings = GetAllBookings();

        for (var i = 0; i < bookings.Count; i++)
            if (bookings[i].Id == updatedBooking.Id)
            {
                bookings[i] = updatedBooking;
                break;
            }

        SaveAll(bookings);
    }

    public void Delete(string bookingId)
    {
        var bookings = GetAllBookings();

        bookings.RemoveAll(f => f.Id == bookingId);

        SaveAll(bookings);
    }

    private void SaveAll(List<Booking> bookings)
    {

        try {
            using var stream = _fileSystem.File.OpenWrite(_filePath);
            using var writer = new StreamWriter(stream);
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(bookings);
            }
            stream.Close();
        } catch (UnauthorizedAccessException ex)
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
