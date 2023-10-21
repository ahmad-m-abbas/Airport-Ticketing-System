using Services.interfaces;
using Views;

namespace Controllers;

public class ManagerController
{
    private readonly IBookingService _bookingService;
    private readonly IFlightService _flightService;
    private readonly ManagerView _managerView;

    public ManagerController(ManagerView managerView, IFlightService flightService, IBookingService bookingService)
    {
        _managerView = managerView;
        _flightService = flightService;
        _bookingService = bookingService;
    }

    public void Run()
    {
        var continueRunning = true;
        while (continueRunning)
        {
            var action = _managerView.DisplayMainMenuAndGetAction();
            switch (action)
            {
                case ManagerAction.FilterBookings:
                    var searchCriteria = _managerView.PromptForBookingFilterCriteria();
                    var filteredBookings = _bookingService.Search(searchCriteria);

                    _managerView.DisplayFilteredBookings(filteredBookings);
                    _managerView.Hold();
                    break;

                case ManagerAction.ImportFlights:
                    var filePath = _managerView.PromptForCSVFilePath();
                    var importStatus = _flightService.ImportFlightsFromCSV(filePath);
                    _managerView.DisplayImportStatus(importStatus);
                    break;

                case ManagerAction.ViewFieldConstraints:
                    var constraints = _flightService.GetFlightDataConstraints();
                    _managerView.DisplayFieldConstraints(constraints);
                    break;

                case ManagerAction.Exit:
                    continueRunning = false;
                    break;

                default:
                    _managerView.DisplayErrorMessage("Invalid action");
                    break;
            }
        }
    }
}