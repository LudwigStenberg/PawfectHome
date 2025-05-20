public interface IUserService
{
    public Task<UserSummaryResponse> GetUserAsync(string id);
}
