using System.Security.Claims;

public interface IUserService
{
    public Task<UserSummaryResponse> GetUserAsync(string id, ClaimsPrincipal currentUser);

    public Task RemoveUserAsync(string id, ClaimsPrincipal currentUser);
}
