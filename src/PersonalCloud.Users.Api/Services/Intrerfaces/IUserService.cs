namespace PersonalCloud.Users.Api.Services.Intrerfaces;

public interface IUserService
{
  Task<string> RegisterUser(string username, string email, string password, string firstName, string lastName);
  Task<string> LoginUser(string username, string password);
}