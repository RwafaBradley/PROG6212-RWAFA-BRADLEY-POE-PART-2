using CMCS.Models;
namespace CMCS.Services;
public interface IUserService
{
    UserDto? ValidateCredentials(string username, string password);
    IEnumerable<UserDto> GetAllUsers();
}