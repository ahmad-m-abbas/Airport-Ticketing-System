using Models;

namespace Services;

public class CurrentUserService : ICurrentUserService
{
    public User CurrentUser { get; set; }
}