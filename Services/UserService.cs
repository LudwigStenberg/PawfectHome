using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly AppDbContext appDbContext;
    private readonly ILogger<IUserService> logger;
    private readonly IUserRepository userRepository;

    public UserService(
        IUserRepository userRepository,
        ILogger<UserService> logger,
        AppDbContext appDbContext
    )
    {
        this.appDbContext = appDbContext;
        this.logger = logger;
        this.userRepository = userRepository;
    }

    public async Task<UserSummaryResponse> GetUserAsync(int id)
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
