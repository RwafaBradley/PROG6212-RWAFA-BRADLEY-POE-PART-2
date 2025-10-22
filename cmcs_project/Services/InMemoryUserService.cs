using CMCS.Models;

namespace CMCS.Services;
public class InMemoryUserService : IUserService
{
  
    private readonly List<UserDto> _users = new()
    {
        new UserDto { Username = "lecturer1", Password = "password", Role = "Lecturer" },
        new UserDto { Username = "coordinator1", Password = "password", Role = "Coordinator" },
        new UserDto { Username = "manager1", Password = "password", Role = "Manager" }
    };

    public UserDto? ValidateCredentials(string username, string password)
    {
        return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                          && u.Password == password);
    }

    public IEnumerable<UserDto> GetAllUsers() => _users;
}