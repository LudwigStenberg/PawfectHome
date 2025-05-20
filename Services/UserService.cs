using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly ILogger<IUserService> logger;
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        this.logger = logger;
        this.userRepository = userRepository;
    }

    /// <summary>
    /// Recieves a request for fetching a specific user.
    /// </summary>
    /// <param name="id">Used to identify what user to fetch</param>
    /// <returns> User with meta data related to the specific user</returns>
    /// <exception cref="KeyNotFoundException"> If not found throw exception</exception>

    public async Task<UserSummaryResponse> GetUserAsync(string id)
    {
        var user = await userRepository.FetchUserAsync(id);

        if (user == null)
        {
            logger.LogWarning("User with id {userId} was not found", id);
            throw new KeyNotFoundException("User not found");
        }
        var response = new UserSummaryResponse
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
        };
        return response;
    }
}
