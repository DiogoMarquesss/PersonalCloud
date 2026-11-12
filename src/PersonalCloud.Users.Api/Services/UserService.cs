
using PersonalCloud.Users.Data.Entities;
using PersonalCloud.Users.Api.Services.Intrerfaces;
using Microsoft.AspNetCore.Identity;

namespace PersonalCloud.Users.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
        
    }

    public async Task<string> LoginUser(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            throw new Exception("Invalid username or password.");
        }

        return user.Id;
    }
    public async Task<string> RegisterUser(string username, string email, string password, string firstName, string lastName)
    {
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = username,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return user.Id;
    }

}
