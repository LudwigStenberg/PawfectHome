public interface IUserService
{
    public Task<UserSummaryResponse> GetUserAsync(int id);
}
