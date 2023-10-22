using Models;

namespace Services.interfaces;

public interface IBookingService
{
    List<Booking> GetAll();
    Booking GetBookingById(string id);
    void UpdateBooking(Booking nBooking);
    void DeleteBooking(string id);
    Booking BookFlight(Booking booking);
    List<BookingDetails> Search(BookingSearchParams searchParams);
}