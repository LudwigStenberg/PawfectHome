public class UserService : IUserService
{
    public UserService(IUserRepository userRepository, ILogger logger, AppDbContext appDbContext);

    public async Task<GetUserReponse> GetUserAsync(int id)
    {
        var user = await userRepository.FetchUserAsync(id);
    }
}
