using Models;
using Services;
using Services.interfaces;
using Views;

namespace Controllers;

public class PassengerController
{
    private readonly IBookingService _bookingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFlightService _flightService;
    private readonly PassengerView _passengerView;
    private readonly IUserService _userService;

    public PassengerController(PassengerView passengerView, IFlightService flightService,
        IBookingService bookingService, IUserService userService, ICurrentUserService currentUserService)
    {
        _passengerView = passengerView;
        _flightService = flightService;
        _bookingService = bookingService;
        _userService = userService;
        _currentUserService = currentUserService;
    }

    public void Run()
    {
        var continueRunning = true;
        while (continueRunning)
        {
            var action = _passengerView.DisplayMainMenuAndGetAction();
            var userBookings = _userService.GetBookings(_currentUserService.CurrentUser.ID);
            var allFlights = _flightService.SearchFlights(new FlightSearchParams());
            switch (action)
            {
                case PassengerAction.SearchFlights:
                    var searchCriteria = _passengerView.PromptForFlightSearchCriteria();
                    var availableFlights = _flightService.SearchFlights(searchCriteria);

                    _passengerView.DisplayAvailableFlights(availableFlights);
                    _passengerView.Hold();
                    break;

                case PassengerAction.BookFlight:
                    var bookCriteria = _passengerView.PromptForFlightSearchCriteria();
                    var availableFlightsToBook = _flightService.SearchFlights(bookCriteria);
                    var booking = _passengerView.PromptForFlightToBook(availableFlightsToBook);

                    _bookingService.BookFlight(booking);
                    userBookings = _userService.GetBookings(_currentUserService.CurrentUser.ID);
                    _passengerView.DisplayUserBookingStatus(userBookings);
                    break;

                case PassengerAction.ViewBookings:
                    _passengerView.DisplayUserBookingStatus(userBookings);
                    _passengerView.Hold();
                    break;

                case PassengerAction.CancelBooking:
                    _passengerView.DisplayUserBookingStatus(userBookings);
                    var bookingId = _passengerView.PromptForBookingToCancel(userBookings);
                    _bookingService.DeleteBooking(bookingId);
                    _passengerView.DisplayCancellationStatus();
                    break;

                case PassengerAction.ModifyBooking:
                    var bookingToModify = _passengerView.PromptForBookingDetailsToModify(userBookings, allFlights);
                    if (bookingToModify == null) break;
                    _bookingService.UpdateBooking(bookingToModify);
                    _passengerView.DisplayModificationStatus();
                    break;

                case PassengerAction.Exit:
                    continueRunning = false;
                    break;

                default:
                    _passengerView.DisplayErrorMessage("Invalid action");
                    break;
            }
        }
    }
}