using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models;
using Services.exceptions;
using Services.interfaces;

namespace Services;

public class UserService : IUserService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightService _flightService;
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IFlightService flightService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _flightService = flightService;
        _logger = logger;
    }

    public List<User> GetAll()
    {
        return _userRepository.GetAllUsers();
    }

    public User? GetUserById(string id)
    {
        var passengers = _userRepository.GetPassengers(new UserSearchParams { ID = id });

        if (passengers.Count == 0) throw new PassengerNotFoundException($"There is not Passenger with id = {id}");

        return passengers[0];
    }

    public User AuthenticateUser(string email, string password)
    {
        var passengers = _userRepository.GetPassengers(new UserSearchParams { Email = email, Password = password });

        if (passengers.Count == 0) return null;

        return passengers[0];
    }

    public void UpdateUser(User user)
    {
        UpdateUser(user);
    }

    public void DeleteUser(string id)
    {
        DeleteUser(id);
    }

    public List<BookingDetails> GetBookings(string id)
    {
        var bookings = _bookingRepository.GetBookings(new BookingSearchParams { PassengerId = id });
        
        var bookingDetails = bookings.Select(b => BookingDetails.FromSuper(b)).ToList();
        bookingDetails.ForEach(b => b.Flight = _flightService.GetFlightById(b.FlightId));
        bookingDetails.ForEach(b => b.User = GetUserById(b.PassengerId));

        return bookingDetails;
    }

    public void AddUser(User user)
    {
        if (user.ID == null) user.ID = Guid.NewGuid().ToString();
        _userRepository.SaveUsers(new List<User> { user });
    }
}