using Microsoft.AspNetCore.Identity;

public class UserService : IUserService
{
    private readonly ILogger<IUserService> logger;
    private readonly IUserRepository userRepository;
    private readonly UserManager<UserEntity> userManager;
    private readonly ModelValidator modelValidator;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, UserManager<UserEntity> userManager, ModelValidator modelValidator)
    {
        this.logger = logger;
        this.userRepository = userRepository;
        this.userManager = userManager;
        this.modelValidator = modelValidator;
    }

    /// <summary>
    /// Asynchronously registers a new user in the system with validation and error handling.
    /// </summary>
    /// <param name="request">The registration request containing user details (email, password, names).</param>
    /// <returns>A RegisterUserResponse containing the newly created user's information.</returns>
    /// <exception cref="ValidationFailedException">
    /// Thrown when the request model fails validation (invalid email format, missing required fields, etc.) 
    /// or when user creation fails (duplicate email, weak password, etc.).
    /// </exception>
    public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request)
    {
        logger.LogInformation("Starting user registration for email: {Email}", request.Email);

        modelValidator.ValidateModel(request);

        var user = UserMapper.ToEntity(request);

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning("User registration failed for email: {Email}. Errors: {Errors}",
            request.Email,
            string.Join(", ", result.Errors.Select(e => e.Description)));

            var errors = result.Errors.Select(e => new ValidationError("Registration", e.Description));
            throw new ValidationFailedException("User registration failed", errors);
        }

        logger.LogInformation("User successfully registered with ID: '{UserId}'", user.Id);
        return UserMapper.ToRegisterResponse(user);
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
            throw new UserNotFoundException("User not found");
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
            throw new UserNotFoundException("User not found");
        }
        await userRepository.DeleteUserAsync(id);
    }
}
