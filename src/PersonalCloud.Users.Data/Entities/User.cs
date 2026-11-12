using Microsoft.AspNetCore.Identity;

namespace PersonalCloud.Users.Data.Entities;
public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}