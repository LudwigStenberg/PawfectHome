using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ShelterService : IShelterService
{
    private readonly ILogger<ShelterService> logger;
    private readonly IShelterRepository shelterRepository;
    private readonly UserManager<UserEntity> userManager;
    private readonly ModelValidator modelValidator;

    public ShelterService(
        ILogger<ShelterService> logger,
        IShelterRepository shelterRepository,
        UserManager<UserEntity> userManager,
        ModelValidator modelValidator
    )
    {
        this.logger = logger;
        this.shelterRepository = shelterRepository;
        this.userManager = userManager;
        this.modelValidator = modelValidator;
    }

    /// <summary>
    /// Asynchronously registers a new shelter for a user in the system. Enforces the business rule that a user
    /// can only have one shelter at a time. Additionally, assigns the user with a new role: "ShelterOwner" in a non-atomic operation.
    /// </summary>
    /// <param name="userId">The ID of the user that has requested the shelter registration.</param>
    /// <param name="request">The request DTO which contains properties for 'Name', 'Description' and 'Email'.</param>
    /// <returns>
    /// A tuple containing:
    ///   - Shelter: A RegisterShelterResponse DTO with the shelter's 'Id', 'Name', 'Description', 'Email' and the 'UserId'.
    ///   - AuthChanged: A boolean indicating whether the user's authentication state was changed by assigning the ShelterOwner role.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when userId is null or empty, or when the request object is null.</exception>
    /// <exception cref="MultipleSheltersNotAllowedException">Thrown when a user who already has a shelter attempts to register another one. Each user can only have one shelter at a time.</exception>
    public async Task<(RegisterShelterResponse Shelter, bool AuthChanged)> RegisterShelterAsync(
        string userId,
        RegisterShelterRequest request
    )
    {
        logger.LogInformation("Starting shelter registration for user {UserId}.", userId);

        ValidateRegisterShelterRequest(userId, request);

        bool existingShelter = await shelterRepository.DoesShelterExistForUserAsync(userId);
        if (existingShelter)
        {
            logger.LogWarning("User {UserId} attempted to register a second shelter.", userId);
            throw new MultipleSheltersNotAllowedException(userId);
        }

        var newShelter = ShelterMapper.ToEntity(request, userId);

        logger.LogInformation(
            "Creating new shelter for user {UserId} with name {ShelterName}.",
            userId,
            newShelter.Name
        );

        var createdShelter = await shelterRepository.CreateShelterAsync(newShelter);
        bool authChanged = await AssignShelterOwnerRoleAsync(userId);

        var shelter = ShelterMapper.ToRegisterResponse(createdShelter);

        logger.LogInformation(
            "Successfully created shelter {ShelterId} for user {UserId}.",
            createdShelter.Id,
            userId
        );

        return (Shelter: shelter, AuthChanged: authChanged);
    }

    /// <summary>
    /// Asynchronously retrieves information about a shelter based on the shelter ID provided. Also includes a collection of pets associated with that shelter.
    /// </summary>
    /// <param name="id">The ID of the shelter that is used in the retrieval request.</param>
    /// <returns>A ShelterResponse DTO which includes basic information about the shelter in addition to a list of PetSummaryResponse associated with it.</returns>
    /// <exception cref="ShelterNotFoundException">Thrown when the shelter cannot be found.</exception>
    public async Task<ShelterDetailResponse> GetShelterAsync(int id)
    {
        logger.LogInformation(
            "Starting retrieval of shelter information for shelter with ID: {ShelterId}.",
            id
        );

        var shelter = await shelterRepository.FetchShelterByIdAsync(id);

        if (shelter == null)
        {
            logger.LogWarning("Shelter with ID: {ShelterId} could not be found.", id);
            throw new ShelterNotFoundException(id);
        }

        var response = ShelterMapper.ToDetailResponse(shelter);
        return response;
    }

    /// <summary>
    /// Asynchronously retrieves a collection of summarized information for all shelters.
    /// </summary>
    /// <returns>A collection of ShelterSummaryResponse objects. This collection may be empty if no shelters exist in the system.</returns>
    public async Task<ICollection<ShelterSummaryResponse>> GetAllSheltersAsync()
    {
        var allShelters = await shelterRepository.FetchAllSheltersAsync();

        logger.LogInformation("Retrieved {Count} shelters from the repository", allShelters.Count);

        var response = allShelters.Select(ShelterMapper.ToSummaryResponse).ToList();
        return response;
    }

    /// <summary>
    /// Updates a shelter entity using a ShelterUpdateRequest DTO containing nullable properties,
    /// enabling partial updates to the shelter model.
    /// </summary>
    /// </summary>
    /// <param name="id">The ID of the shelter resource to be updated.</param>
    /// <param name="userId">The ID of the user requesting the update.</param>
    /// <param name="request">The ShelterUpdateRequest DTO which contains the new value or values.</param>
    /// <returns>A ShelterDetailResponse DTO containing Id, Name, Description, Email, UserId and a list of Pets.</returns>
    /// <exception cref="ShelterNotFoundException">Thrown when FetchShelterById method fails and the shelter cannot be found.</exception>
    /// <exception cref="ShelterOwnershipException">Thrown when the retrieved shelter's UserId does not match the userId method parameter.</exception>
    public async Task<ShelterDetailResponse> UpdateShelterAsync(
        int id,
        string userId,
        ShelterUpdateRequest request
    )
    {
        logger.LogInformation(
            "Starting update for shelter with ID: {ShelterId}. Update request made by user ID: {RequestingUserId}",
            id,
            userId
        );

        ValidateShelterUpdateRequest(userId, request);

        var existingShelter = await shelterRepository.FetchShelterByIdAsync(id);
        if (existingShelter == null)
        {
            logger.LogWarning("The shelter with ID: {ShelterId} could not be found", id);
            throw new ShelterNotFoundException(id);
        }

        if (existingShelter.UserId != userId)
        {
            logger.LogWarning(
                "Authorization failure: User {RequestingUserId} attempted to update shelter {ShelterId} owned by user {OwnerUserId}.",
                userId,
                existingShelter.Id,
                existingShelter.UserId
            );
            throw new ShelterOwnershipException(id, userId);
        }

        if (request.Name != null)
        {
            existingShelter.Name = request.Name;
        }

        if (request.Description != null)
        {
            existingShelter.Description = request.Description;
        }

        if (request.Email != null)
        {
            existingShelter.Email = request.Email;
        }

        logger.LogDebug(
            "Updating shelter {ShelterId}: Name={NameUpdated}, Description={DescriptionUpdated}, Email={EmailUpdated}",
            existingShelter.Id,
            request.Name != null,
            request.Description != null,
            request.Email != null
        );

        await shelterRepository.UpdateShelterAsync(existingShelter);

        logger.LogDebug(
            "The update for shelter with ID: {ShelterId} was successful. Returning a new ShelterDetailResponse.",
            existingShelter.Id
        );

        var response = ShelterMapper.ToDetailResponse(existingShelter);
        return response;
    }

    /// <summary>
    ///  Removes a shelter based on the shelter ID passed as an argument. Retrieves the shelter entity to make sure
    ///  that the shelter belongs to the user ID which is also passed as an argument. After the deletion of a shelter
    ///  the user's role 'ShelterOwner' will be unassigned.
    /// </summary>
    /// <param name="id">The ID of the shelter resource to be deleted.</param>
    /// <param name="userId">The ID of the user requesting the deletion.</param>
    /// <returns>Returns a Task representing the asynchronous operation. No data is returned upon completion.</returns>
    /// <exception cref="ShelterNotFoundException">Thrown when the retrieved shelter is null. The shelter could not be found.</exception>
    /// <exception cref="ShelterOwnershipException">Thrown when the User ID associated with the shelter does not match the user ID that was passed in.</exception>
    /// <remarks>
    /// This method will trigger cascade deletion of all pets associated with the shelter due to database configuration.
    /// However, adoption applications related to those pets will remain in the database to preserve historical data.
    /// The method performs ownership verification before deletion to ensure users can only delete their own shelters.
    /// </remarks>
    public async Task RemoveShelterAsync(int id, string userId)
    {
        logger.LogInformation(
            "Starting deletion of shelter with ID: {ShelterId}. Deletion request made by user ID: {RequestingUserId}",
            id,
            userId
        );

        var shelter = await shelterRepository.FetchShelterByIdAsync(id);
        if (shelter == null)
        {
            logger.LogWarning("The shelter with ID: {ShelterId} could not be found", id);
            throw new ShelterNotFoundException(id);
        }

        if (shelter.UserId != userId)
        {
            logger.LogWarning(
                "Authorization failure: User {RequestingUserId} attempted to delete shelter {ShelterId} owned by user {UserId}",
                userId,
                id,
                shelter.UserId
            );
            throw new ShelterOwnershipException(id, userId);
        }

        logger.LogDebug(
            "Deleting shelter {ShelterId} with {PetCount} associated pets.",
            id,
            shelter.Pets.Count
        );

        await shelterRepository.DeleteShelterAsync(shelter);

        logger.LogDebug(
            "Successfully deleted shelter with ID: {ShelterId} belonging to user {UserId}.",
            id,
            userId
        );

        await TryRemoveShelterOwnerRoleAsync(userId);
    }

    #region Helper Methods

    /// <summary>
    /// Assigns the "ShelterOwner" role to a specified user and indicates whether the authentication state changed.
    /// </summary>
    /// <param name="userId">The ID of the user to assign the role to.</param>
    /// <returns>
    /// A boolean value indicating whether the authentication state was successfully changed:
    /// - true: The ShelterOwner role was successfully assigned, changing the user's authentication state.
    /// - false: No change occurred because the user wasn't found or the role assignment failed.
    /// </returns>
    /// <remarks>
    /// This method will log a warning if the user is not found or if the role assignment fails,
    /// but it will not throw exceptions. This allows the shelter registration process to complete
    /// even if role assignment fails.
    /// </remarks>
    private async Task<bool> AssignShelterOwnerRoleAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning(
                "User {UserId} not found when trying to assign ShelterOwner role.",
                userId
            );
            return false;
        }

        bool alreadyHasRole = await userManager.IsInRoleAsync(user, "ShelterOwner");
        if (alreadyHasRole)
        {
            logger.LogInformation(
                "User {UserId} already has ShelterOwner role. No change needed.",
                userId
            );
            return false;
        }

        logger.LogInformation("Assigning ShelterOwner role to user {UserId}.", userId);
        var roleResult = await userManager.AddToRoleAsync(user, "ShelterOwner");

        if (!roleResult.Succeeded)
        {
            var errorMessage = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            logger.LogWarning(
                "Failed to assign ShelterOwner role to user {UserId}. Errors: {Errors}",
                userId,
                errorMessage
            );
            return false;
        }

        return true;
    }

    /// <summary>
    ///  Attempts to remove the ShelterOwner role from a user with retry capability.
    /// </summary>
    /// <param name="userId">The ID of the user from whom to remove the ShelterOwner role.</param>
    /// <remarks>
    /// This method will attempt to remove the role 3 times, exponentially increasing the delay (ms)
    /// between each try. The operation is considered complete if any attempt succeeds or after all retry attempts
    /// have been exhausted.
    /// </remarks>
    private async Task TryRemoveShelterOwnerRoleAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning(
                "Could not find user with ID: {UserId} to remove ShelterOwner role",
                userId
            );
            return;
        }

        const int maxRetries = 3;
        bool roleRemoved = false;
        int attempt = 0;

        while (!roleRemoved && attempt < maxRetries)
        {
            attempt++;
            logger.LogDebug(
                "Attempt {Attempt} to remove 'ShelterOwner' role from user {UserId}",
                attempt,
                userId
            );

            var roleResult = await userManager.RemoveFromRoleAsync(user, "ShelterOwner");
            roleRemoved = roleResult.Succeeded;

            if (roleRemoved)
            {
                logger.LogDebug(
                    "Successfully removed 'ShelterOwner' role from user {UserId}",
                    userId
                );
                return;
            }

            if (attempt < maxRetries)
            {
                int delayMilliseconds = 100 * (int)Math.Pow(2, attempt - 1);
                logger.LogWarning(
                    "Failed to remove ShelterOwner role. Retrying in {Delay}ms...",
                    delayMilliseconds
                );
                await Task.Delay(delayMilliseconds);
            }
            else
            {
                logger.LogError(
                    "All attempts to remove ShelterOwner role from user {UserId} failed",
                    userId
                );
            }
        }
    }

    /// <summary>
    /// Validates input parameters for shelter creation, ensuring all required data is present and properly formatted.
    /// This is a private helper method called by RegisterShelterAsync().
    /// </summary>
    /// <param name="userId">The string userId which needs to be checked for null and empty.</param>
    /// <param name="request">The request DTO that needs to be validated based on format of the Email provided, null and white space, Name.Length and Description.Length.</param>
    /// <exception cref="UserIdRequiredException">Thrown when the userId is null or empty.</exception>
    /// <exception cref="ValidationFailedException">Thrown from within ValidateModel method when the validation for the request based on data annotations fails.</exception>
    private void ValidateRegisterShelterRequest(string userId, RegisterShelterRequest request)
    {
        logger.LogDebug("Validating shelter registration request for user {UserId}", userId);

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Shelter registration rejected: User ID is null or empty");
            throw new UserIdRequiredException();
        }

        modelValidator.ValidateModel(request);
    }

    /// <summary>
    /// Validates input parameters for shelter creation, ensuring all required data is present and properly formatted.
    /// This is a private helper method called by UpdateShelterAsync().
    /// </summary>
    /// <param name="userId">The string userId which needs to be checked for null and empty.</param>
    /// <param name="request">The request DTO that needs to be validated based on format of the Email provided, null and white space, Name.Length and Description.Length.</param>
    /// <exception cref="UserIdRequiredException">Thrown when the userId is null or empty.</exception>
    /// <exception cref="ValidationFailedException">Thrown when all of the nullable property fields of the request DTO are null or when the validation within ValidateModel fails.</exception>
    private void ValidateShelterUpdateRequest(string userId, ShelterUpdateRequest request)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Shelter registration rejected: User ID is null or empty.");
            throw new UserIdRequiredException();
        }

        if (request.Name == null && request.Description == null && request.Email == null)
        {
            logger.LogWarning("Shelter update rejected: No properties specified for update.");
            throw CreateValidationFailure("At least one property must be specified for update.");
        }

        modelValidator.ValidateModel(request);
    }

    #endregion
}
