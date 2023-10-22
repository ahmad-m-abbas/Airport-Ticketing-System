using Models;

namespace Data.Interfaces;

public interface IBookingRepository
{
    List<Booking> GetAllBookings();
    void SaveBookings(List<Booking> bookings);
    List<Booking> GetBookings(BookingSearchParams searchParams);
    void Update(Booking updatedBooking);
    void Delete(string bookingId);
}