using System.Security.Claims;
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
    /// Recieves a request for fetching a specific user. Compare with id from logged in user using ClaimsPrinciple.
    /// </summary>
    /// <param name="id">Used to identify what user to fetch</param>
    /// <param name="currentUser">The user logged in </param>
    /// <returns> User with meta data related to the specific user</returns>
    /// <exception cref="UnauthorizedAccessException">If current user is not same as the user being fetched throw an unauthoization exception</exception>
    /// <exception cref="KeyNotFoundException">If not found throw exception</exception> <summary>

    public async Task<UserSummaryResponse> GetUserAsync(string id, string userId)
    {
        var sameUser = userId == id;

        if (!sameUser)
        {
            logger.LogWarning("Current user does not have permission to access. ");
            throw new UnauthorizedAccessException("You can only access your own information");
        }

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
            Email = user.Email!,
        };
        return response;
    }

    /// <summary>
    /// Recieves a request for fetching a specific user corresponding with sent id, if same as current user, delete user from db.
    /// </summary>
    /// <param name="id">Used to identify what user to fetch</param>
    /// <param name="currentUser">The user logged in  </param>
    /// <returns></returns>

    public async Task RemoveUserAsync(string id, string userId)
    {
        var sameUser = userId == id;

        if (!sameUser)
        {
            logger.LogWarning("Current user does not have permission to access. ");
            throw new UnauthorizedAccessException("You can only access your own information");
        }
        var user = await userRepository.FetchUserAsync(id);

        if (user == null)
        {
            logger.LogWarning("User with id {userId} was not found", id);
            throw new KeyNotFoundException("User not found");
        }
        await userRepository.DeleteUserAsync(id);
    }
}
