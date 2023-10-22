using System.Globalization;
using Models;

namespace Views;

public class ManagerView
{
    public ManagerAction DisplayMainMenuAndGetAction()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("Welcome, Manager!");
            Console.WriteLine("Please choose an action:");
            Console.WriteLine("1. Filter Bookings");
            Console.WriteLine("2. Import Flights from CSV");
            Console.WriteLine("3. View Flight Data Constraints");
            Console.WriteLine("4. Exit");

            Console.Write("Enter your choice (1-4): ");

            if (int.TryParse(Console.ReadLine(), out var choice))
                if (choice >= 1 && choice <= 4)
                    return (ManagerAction)choice;

            Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
        }
    }

    public BookingSearchParams PromptForBookingFilterCriteria()
    {
        var filterCriteria = new BookingSearchParams();

        Console.WriteLine("Enter Filter Criteria for Bookings:");
        Console.WriteLine("(You can leave any field blank if you do not wish to filter by it)");

        Console.Write("Booking ID: ");
        filterCriteria.Id = Console.ReadLine();

        Console.Write("Flight ID: ");
        filterCriteria.FlightId = Console.ReadLine();

        Console.Write("Passenger ID: ");
        filterCriteria.PassengerId = Console.ReadLine();

        Console.Write("Passenger Name: ");
        filterCriteria.PassengerName = Console.ReadLine();

        Console.Write("Class (1 for Economy, 2 for Business, 3 for FirstClass or leave blank): ");
        if (int.TryParse(Console.ReadLine(), out var choice) && choice >= 1 && choice <= 3)
            filterCriteria.ClassType = (ClassType)choice;

        Console.Write(
            "Booking Status (1 for Active, 2 for Cancelled, etc. or leave blank): "); // Assuming you have some enumeration for Status
        if (int.TryParse(Console.ReadLine(), out var statusChoice))
            filterCriteria.Status =
                (Status)statusChoice; // Ensure your Status enum corresponds to the numbers provided to the user

        Console.Write("Booking Start Date (yyyy-MM-dd or leave blank): ");
        if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var startDate)) filterCriteria.StartDate = startDate;

        Console.Write("Booking End Date (yyyy-MM-dd or leave blank): ");
        if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var endDate)) filterCriteria.EndDate = endDate;

        return filterCriteria;
    }


    public void DisplayFilteredBookings(List<BookingDetails> bookings)
    {
        for (var i = 0; i < bookings.Count; i++) Console.WriteLine($"({i + 1}) : {bookings[i]}");
    }

    public void Hold()
    {
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    public string PromptForCSVFilePath()
    {
        Console.Write("Enter the path to the CSV file for flight import: ");
        return Console.ReadLine();
    }

    public void DisplayImportStatus(ImportResult status)
    {
        if (status.Status == ImportStatus.Success)
        {
            Console.WriteLine("Flights successfully imported!");
        }
        else
        {
            Console.WriteLine("There were errors during the import:");
            foreach (var error in status.Errors) Console.WriteLine($"- {error}");
        }

        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    public void DisplayFieldConstraints(Dictionary<string, List<string>> constraints)
    {
        foreach (var constraint in constraints)
        {
            Console.WriteLine($"{constraint.Key}:");
            foreach (var description in constraint.Value) Console.WriteLine($"- {description}");
            Console.WriteLine();
        }

        Hold();
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