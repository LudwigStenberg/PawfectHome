using System.Security.Claims;

public interface IUserService
{
    public Task<UserSummaryResponse> GetUserAsync(string id, string userId);

    public Task RemoveUserAsync(string id, string userId);
}
