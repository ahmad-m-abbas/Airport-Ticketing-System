using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models;
using Services.exceptions;
using Services.interfaces;

namespace Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightService _flightService;
    private readonly ILogger<BookingService> _logger;
    private readonly IUserService _userService;

    public BookingService(IBookingRepository bookingRepository,
        IFlightService flightService,
        IUserService userService,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _flightService = flightService;
        _userService = userService;
        _logger = logger;
    }

    public List<Booking> GetAll()
    {
        return _bookingRepository.GetAllBookings();
    }

    public Booking GetBookingById(string id)
    {
        var bookings = _bookingRepository.GetBookings(new BookingSearchParams { PassengerId = id });

        if (bookings.Count == 0) throw new Exception($"There is no Passenger with the id = {id}");

        return bookings[0];
    }

    public Booking BookFlight(Booking bookingDetails)
    {
        try
        {
            var checkBookingExist = _bookingRepository.GetBookings(new BookingSearchParams
                { PassengerId = bookingDetails.PassengerId, FlightId = bookingDetails.FlightId });

            if (checkBookingExist.Count > 0)
                throw new BookingAlreadyExists(bookingDetails.PassengerId, bookingDetails.FlightId);

            var flight = _flightService.GetFlightById(bookingDetails.FlightId);
            var passenger = _userService.GetUserById(bookingDetails.PassengerId);

            if ((bookingDetails.ClassType == ClassType.BUSINESS && flight.AvailableSeatsBusiness == 0)
                || (bookingDetails.ClassType == ClassType.ECONOMY && flight.AvailableSeatsEconomy == 0)
                || (bookingDetails.ClassType == ClassType.FIRST_CLASS && flight.AvailableSeatsFirstClass == 0))
                throw new NoSeasAvailableException(bookingDetails.FlightId);


            _bookingRepository.SaveBookings(new List<Booking> { bookingDetails });
            if (bookingDetails.ClassType == ClassType.FIRST_CLASS) flight.AvailableSeatsFirstClass -= 1;

            if (bookingDetails.ClassType == ClassType.BUSINESS) flight.AvailableSeatsBusiness -= 1;

            if (bookingDetails.ClassType == ClassType.ECONOMY) flight.AvailableSeatsEconomy -= 1;

            _flightService.Update(flight);
            bookingDetails.Status = Status.COMPLETED;

            return bookingDetails;
        }
        catch (FlightNotFoundException ex)
        {
            _logger.LogError(ex,
                $"Error occurred while booking a flight, no flight with the id {bookingDetails.FlightId} exists");
        }
        catch (PassengerNotFoundException ex)
        {
            _logger.LogError(ex,
                $"Error occurred while booking a flight, no passenger with the id {bookingDetails.PassengerId} exists");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while booking a flight");
        }

        return null;
    }

    public void DeleteBooking(string id)
    {
        _bookingRepository.Delete(id);
    }

    public void UpdateBooking(Booking booking)
    {
        _bookingRepository.Update(booking);
    }

    public List<BookingDetails> Search(BookingSearchParams searchParams)
    {
        var allBookings = _bookingRepository.GetBookings(searchParams);
        if (!string.IsNullOrEmpty(searchParams.PassengerName))
            allBookings = allBookings.Where(b =>
                 _userService.GetUserById(b.PassengerId).Name
                    .Contains(searchParams.PassengerName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (searchParams.StartDate.HasValue)
            allBookings = allBookings.Where(b =>
                _flightService.GetFlightById(b.FlightId).DepartureDate >= searchParams.StartDate.Value).ToList();
        if (searchParams.EndDate.HasValue)
            allBookings = allBookings.Where(b =>
                _flightService.GetFlightById(b.FlightId).DepartureDate <= searchParams.EndDate.Value).ToList();

        var bookingDetails = allBookings.Select(BookingDetails.FromSuper).ToList();
        bookingDetails.ForEach(b => b.Flight = _flightService.GetFlightById(b.FlightId));
        bookingDetails.ForEach(b => b.User = _userService.GetUserById(b.PassengerId));

        return bookingDetails;
    }
}