namespace PersonalCloud.Users.Api.DTOs.Post;

public record LoginUserRequest(
    string Username,
    string Password
);