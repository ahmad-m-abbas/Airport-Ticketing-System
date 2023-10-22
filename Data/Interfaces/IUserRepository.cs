using Models;

namespace Data.Interfaces;

public interface IUserRepository
{
    List<User> GetAllUsers();
    void SaveUsers(List<User> users);
    List<User> GetPassengers(UserSearchParams searchParams);
    void Update(User updatedUser);
    void Delete(string userId);
}