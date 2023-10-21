namespace Models;

public class BookingDetails : Booking
{
    public Flight Flight { get; set; }
    public User User { get; set; }

    public static BookingDetails FromSuper(Booking booking)
    {
        var bookingDetails = new BookingDetails();
        bookingDetails.Id = booking.Id;
        bookingDetails.FlightId = booking.FlightId;
        bookingDetails.PassengerId = booking.PassengerId;
        bookingDetails.Status = booking.Status;

        return bookingDetails;
    }

    public override string ToString()
    {
        return
            $"Flight \t {Flight.DepartureAirport} => {Flight.ArrivalAirport} at {Flight.DepartureDate} with status {Status} for the passenger {User.Name}";
    }
}