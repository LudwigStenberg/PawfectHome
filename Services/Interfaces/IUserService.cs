public interface IUserService
{
    Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest newUser);
    Task<UserSummaryResponse> GetUserAsync(string id, string userId);
    Task RemoveUserAsync(string id, string userId);
}
