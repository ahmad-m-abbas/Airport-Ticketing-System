using System.Globalization;
using System.IO.Abstractions;
using CsvHelper;
using CsvHelper.Configuration;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models;

namespace Data;

public class UserRepository : IUserRepository
{
    
    private readonly IFileSystem _fileSystem;
    private readonly string _filePath;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(string filePath, IFileSystem fileSystem, ILogger<UserRepository> logger)
    {
        _filePath = filePath;
        _fileSystem = fileSystem;
        _logger = logger;
    }
    
    public List<User> GetAllUsers()
    {
        try
        {
            if (!_fileSystem.File.Exists(_filePath))
                return new List<User>();

            using var fileStream = _fileSystem.File.OpenRead(_filePath);
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var users = csv.GetRecords<User>().ToList();
            return users;
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

    public void SaveUsers(List<User> passengers)
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
                csv.WriteHeader<User>();
                csv.NextRecord();
            }

            csv.WriteRecords(passengers);
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

    public List<User> GetPassengers(UserSearchParams searchParams)
    {
        var allPassengers = GetAllUsers();

        if (!string.IsNullOrEmpty(searchParams.ID))
            allPassengers = allPassengers.Where(p => p.ID == searchParams.ID).ToList();

        if (!string.IsNullOrEmpty(searchParams.Name))
            allPassengers = allPassengers
                .Where(p => p.Name.Contains(searchParams.Name, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(searchParams.Email))
            allPassengers = allPassengers
                .Where(p => p.Email.Contains(searchParams.Email, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(searchParams.Password))
            allPassengers = allPassengers
                .Where(p => p.Password.Contains(searchParams.Password, StringComparison.OrdinalIgnoreCase)).ToList();

        return allPassengers;
    }

    public void Update(User updatedUser)
    {
        var passengers = GetAllUsers();

        for (var i = 0; i < passengers.Count; i++)
            if (passengers[i].ID == updatedUser.ID)
            {
                passengers[i] = updatedUser;
                break;
            }

        SaveAll(passengers);
    }

    public void Delete(string passengerId)
    {
        var passengers = GetAllUsers();

        passengers.RemoveAll(p => p.ID == passengerId);

        SaveAll(passengers);
    }

    private void SaveAll(List<User> passengers)
    {
        try
        {
            using var stream = _fileSystem.File.OpenWrite(_filePath);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                                                                     
            csv.WriteRecords(passengers);
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