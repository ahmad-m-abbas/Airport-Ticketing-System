using Models;

namespace Services.interfaces;

public interface IUserService
{
    List<User> GetAll();
    User GetUserById(string id);
    User AuthenticateUser(string email, string password);
    void UpdateUser(User user);
    void DeleteUser(string id);
    void AddUser(User user);
    List<BookingDetails> GetBookings(string id);
}