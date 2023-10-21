using System.Globalization;
using Controllers;
using Models;
using Services;

namespace Views;

public class PassengerView
{
    private readonly ICurrentUserService _currentUserService;

    public PassengerView(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public void Hold()
    {
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    public void DisplayAvailableFlights(List<Flight> flights)
    {
        const int flightsPerPage = 5;

        if (flights.Count == 0)
        {
            Console.WriteLine("There are no flights.");
            return;
        }

        for (var i = 0; i < flights.Count; i++)
        {
            Console.WriteLine($"({i + 1}) : {flights[i]}");

            if ((i + 1) % flightsPerPage == 0 && i != flights.Count - 1)
            {
                Console.WriteLine("Enter any key view the next 5 flights or 'E' to exit.");
                var input = Console.ReadKey(true).KeyChar;

                if (char.ToUpper(input) == 'E') break;

                Console.WriteLine();
            }
        }
    }

    public FlightSearchParams PromptForFlightSearchCriteria()
    {
        var searchParams = new FlightSearchParams();

        Console.WriteLine("Enter Flight Search Criteria:");
        Console.WriteLine("(You can leave any field blank if you do not wish to filter by it)");

        Console.Write("Flight ID: ");
        searchParams.Id = Console.ReadLine();

        Console.Write("Departure Country: ");
        searchParams.DepartureCountry = Console.ReadLine();

        Console.Write("Destination Country: ");
        searchParams.DestinationCountry = Console.ReadLine();

        Console.Write("Departure Date (yyyy-MM-dd or leave blank): ");
        if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var date)) searchParams.DepartureDate = date;

        Console.Write("Departure Airport: ");
        searchParams.DepartureAirport = Console.ReadLine();

        Console.Write("Arrival Airport: ");
        searchParams.ArrivalAirport = Console.ReadLine();

        Console.Write("Maximum Price (or leave blank): ");
        if (decimal.TryParse(Console.ReadLine(), out var price)) searchParams.MaxPrice = price;

        Console.Write("Class (1 for Economy, 2 for Business, 3 for FirstClass or leave blank): ");

        if (int.TryParse(Console.ReadLine(), out var choice))
            if (choice >= 1 && choice <= 3)
                searchParams.ClassType = (ClassType)choice;

        return searchParams;
    }

    public Booking PromptForFlightToBook(List<Flight> flights)
    {
        DisplayAvailableFlights(flights);

        Flight flight;
        while (true)
        {
            Console.Write("Flight ID: ");
            if (int.TryParse(Console.ReadLine(), out var Id))
            {
                Id -= 1;
                if (Id >= 0 && Id < flights.Count)
                {
                    flight = flights[Id];
                    break;
                }

                Console.WriteLine("Please Enter a valid number");
            }
            else
            {
                Console.WriteLine("Please Enter a valid Integer");
            }
        }

        if (flight.AvailableSeatsEconomy + flight.AvailableSeatsFirstClass + flight.AvailableSeatsBusiness == 0)
        {
            Console.WriteLine("There is no available seats for this flight");
            return null;
        }

        var booking = new Booking();
        booking.Id = Guid.NewGuid().ToString();
        booking.FlightId = flight.Id;
        booking.PassengerId = _currentUserService.CurrentUser.ID;
        Console.Write("Class (1 for Economy, 2 for Business, 3 for FirstClass): ");


        while (true)
            if (int.TryParse(Console.ReadLine(), out var choice))
            {
                ClassType classType;
                if (choice >= 1 && choice <= 3)
                {
                    classType = (ClassType)choice;
                    if ((classType.Equals(ClassType.BUSINESS) && flight.AvailableSeatsBusiness == 0)
                        || (classType.Equals(ClassType.FIRST_CLASS) && flight.AvailableSeatsFirstClass == 0)
                        || (classType.Equals(ClassType.ECONOMY) && flight.AvailableSeatsEconomy == 0))
                    {
                        Console.WriteLine("Sorry, no remaining seats, try other class");
                    }
                    else
                    {
                        booking.ClassType = classType;
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid number");
                }
            }

        return booking;
    }

    public PassengerAction DisplayMainMenuAndGetAction()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("Welcome, Passenger!");
            Console.WriteLine("Please choose an action:");
            Console.WriteLine("1. Book a Flight");
            Console.WriteLine("2. Search for Available Flights");
            Console.WriteLine("3. Cancel a Booking");
            Console.WriteLine("4. Modify a Booking");
            Console.WriteLine("5. View Your Bookings");
            Console.WriteLine("6. Exit");

            Console.Write("Enter your choice (1-6): ");

            if (int.TryParse(Console.ReadLine(), out var choice))
                if (choice >= 1 && choice <= 6)
                    return (PassengerAction)choice;

            Console.WriteLine("Invalid choice. Please enter a number between 1 and 6.");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
        }
    }

    public void DisplayUserBookingStatus(List<BookingDetails> bookingDetails)
    {
        for (var i = 0; i < bookingDetails.Count; i++) Console.WriteLine($"({i + 1}) : {bookingDetails[i]}");
    }

    public string PromptForBookingToCancel(List<BookingDetails> bookingDetails)
    {
        DisplayUserBookingStatus(bookingDetails);
        Console.WriteLine("Enter Booking Number you want to delete:");

        BookingDetails booking;
        while (true)
        {
            Console.Write("Booking Number: ");
            if (int.TryParse(Console.ReadLine(), out var Id))
            {
                if (Id == 0) return null;
                if (Id >= 1 && Id <= bookingDetails.Count)
                {
                    booking = bookingDetails[Id];
                    break;
                }

                Console.WriteLine("Please Enter a valid number, for exit enter 0");
            }
            else
            {
                Console.WriteLine("Please Enter a valid Integer, for exit enter 0");
            }
        }

        return booking.Id;
    }

    public void DisplayCancellationStatus()
    {
        Console.WriteLine("Your booking has been successfully canceled.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    public Booking PromptForBookingDetailsToModify(List<BookingDetails> bookingDetails, List<Flight> flights)
    {
        DisplayUserBookingStatus(bookingDetails);
        Console.WriteLine("Enter Booking Number you want to update:");
        BookingDetails booking;
        while (true)
        {
            Console.Write("Booking Number: ");
            if (int.TryParse(Console.ReadLine(), out var Id))
            {
                if (Id == 0) return null;

                Id -= 1;

                if (Id >= 0 && Id < bookingDetails.Count)
                {
                    booking = bookingDetails[Id];
                    break;
                }

                Console.WriteLine("Please Enter a valid number, for exit enter 0");
            }
            else
            {
                Console.WriteLine("Please Enter a valid Integer, for exit enter 0");
            }
        }

        Console.WriteLine($"Current Flight ID: {booking.FlightId}");

        var newBooking = PromptForFlightToBook(flights);
        newBooking.Id = booking.Id;

        return newBooking;
    }

    public void DisplayModificationStatus()
    {
        Console.WriteLine("Your booking has been successfully modified.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    public void DisplayErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ERROR: " + message);
        Console.ResetColor();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}