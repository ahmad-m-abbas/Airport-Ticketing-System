using Models;

namespace Services;

public interface ICurrentUserService
{
    User CurrentUser { get; set; }
}