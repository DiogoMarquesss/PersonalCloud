namespace PersonalCloud.Users.Api.DTOs.Post;

public record RegisterUserRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName
);