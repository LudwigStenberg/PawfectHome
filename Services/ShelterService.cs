using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ShelterService : IShelterService
{
    private readonly ILogger<ShelterService> logger;
    private readonly IShelterRepository shelterRepository;
    private readonly UserManager<UserEntity> userManager;

    public ShelterService(
        ILogger<ShelterService> logger,
        IShelterRepository shelterRepository,
        UserManager<UserEntity> userManager
    )
    {
        this.logger = logger;
        this.shelterRepository = shelterRepository;
        this.userManager = userManager;
    }

    /// <summary>
    /// Asyncronously registers a new shelter for a user in the system. Enforces the business rule that a user
    /// can only have one shelter at a time. Additionally, assigns the user with a new role: "ShelterOwner" in a non-atomic operation.
    /// </summary>
    /// <param name="userId">The ID of the user that has requested the shelter registration.</param>
    /// <param name="request">The request DTO which contains properties for 'Name', 'Description' and 'Email'.</param>
    /// <returns>A RegisterShelterDetailResponse DTO which contains the shelter's 'Id', 'Name', 'Description', 'Email' and the 'UserId' for the associated user.</returns>
    /// <exception cref="ArgumentException">Thrown when userId is null or empty, or when the request object is null.</exception>
    /// <exception cref="ValidationException">Thrown when a user who already has a shelter attempts to register another one. Each user can only have one shelter at a time.</exception>
    public async Task<RegisterShelterDetailResponse> RegisterShelterAsync(
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
            throw new ValidationException(
                "User already has a shelter. Each user can only have one shelter registered at a time."
            );
        }

        var newShelter = new ShelterEntity
        {
            Name = request.Name,
            Description = request.Description ?? "No description",
            Email = request.Email,
            UserId = userId
        };

        logger.LogInformation(
            "Creating new shelter for user {UserId} with name {ShelterName}.",
            userId,
            newShelter.Name
        );

        var createdShelter = await shelterRepository.CreateShelterAsync(newShelter);

        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
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
            }
        }
        else
        {
            logger.LogWarning(
                "User {UserId} not found when trying to assign ShelterOwner role.",
                userId
            );
        }

        var response = new RegisterShelterDetailResponse
        {
            Id = createdShelter.Id,
            Name = createdShelter.Name,
            Description = createdShelter.Description,
            Email = createdShelter.Email,
            UserId = createdShelter.UserId,
        };

        logger.LogInformation(
            "Successfully created shelter {ShelterId} for user {UserId}.",
            createdShelter.Id,
            userId
        );
        return response;
    }

    /// <summary>
    /// Asynchronously retrieves information about a shelter based on the shelter ID provided. Also includes a collection of pets associated with that shelter.
    /// </summary>
    /// <param name="id">The ID of the shelter that is used in the retrieval request.</param>
    /// <returns>A ShelterResponse DTO which includes basic information about the shelter in addition to a list of PetResponses associated with it.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the shelter cannot be found.</exception>
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
            throw new KeyNotFoundException($"Shelter with ID {id} could not be found.");
        }

        return new ShelterDetailResponse()
        {
            Id = shelter.Id,
            Name = shelter.Name,
            Description = shelter.Description,
            Email = shelter.Email,
            UserId = shelter.UserId,

            // TODO: Replace the PetResponseTemp with actual PetResponse when it is ready.
            Pets = shelter.Pets.Select(pet => new PetResponseTemp
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species

            }).ToList()
        };
    }

    /// <summary>
    /// Asynchronously retrieves a collection of summarized information for all shelters.
    /// </summary>
    /// <returns>A collection of ShelterSummaryResponse objects. This collection may be empty if no shelters exist in the system.</returns>
    public async Task<ICollection<ShelterSummaryResponse>> GetAllSheltersAsync()
    {
        var allShelters = await shelterRepository.FetchAllSheltersAsync();

        logger.LogInformation("Retrieved {Count} shelters from the repository", allShelters.Count);

        return allShelters.Select(shelter => new ShelterSummaryResponse()
        {
            Id = shelter.Id,
            Name = shelter.Name,
            Description = shelter.Description,
            Email = shelter.Email,
            PetCount = shelter.PetCount

        }).ToList();
    }

    /// <summary>
    ///  Updates a shelter based on the ShelterUpdateRequest which contains nullable property values which allows for partial updating within the model.
    /// </summary>
    /// <param name="id">The ID of the shelter resource to be updated.</param>
    /// <param name="userId">The ID of the user requesting the update.</param>
    /// <param name="request">The ShelterUpdateRequest DTO which contains the new value or values.</param>
    /// <returns>A ShelterDetailResponse DTO containing Id, Name, Description, Email, UserId and a list of Pets.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when FetchShelterById method fails and the shelter cannot be found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the retrieved shelter's UserId does not match the userId method parameter.</exception>
    public async Task<ShelterDetailResponse> UpdateShelterAsync(int id, string userId, ShelterUpdateRequest request)
    {
        logger.LogInformation("Starting update for shelter with ID: {ShelterId}. Update request made by user ID: {RequestingUserId}", id, userId);
        var existingShelter = await shelterRepository.FetchShelterByIdAsync(id);
        if (existingShelter == null)
        {
            logger.LogWarning("The shelter with ID: {ShelterId} could not be found", id);
            throw new KeyNotFoundException($"Shelter with ID {id} could not be found.");
        }

        if (existingShelter.UserId != userId)
        {
            logger.LogWarning("Authorization failure: User {RequestingUserId} attempted to update shelter {ShelterId} owned by user {OwnerUserId}.",
                userId, existingShelter.Id, existingShelter.UserId);
            throw new UnauthorizedAccessException("You do not have permission to update this shelter.");
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

        logger.LogDebug("Updating shelter {ShelterId}: Name={NameUpdated}, Description={DescriptionUpdated}, Email={EmailUpdated}",
            existingShelter.Id,
            request.Name != null,
            request.Description != null,
            request.Email != null);

        await shelterRepository.UpdateShelterAsync(existingShelter);

        logger.LogDebug("The update for shelter with ID: {ShelterId} was successful. Returning a new ShelterDetailResponse.", existingShelter.Id);

        return new ShelterDetailResponse
        {
            Id = existingShelter.Id,
            Name = existingShelter.Name,
            Description = existingShelter.Description,
            Email = existingShelter.Email,
            UserId = existingShelter.UserId,
            Pets = existingShelter.Pets.Select(pet => new PetResponseTemp
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species
            }).ToList()
        };
    }

    /// <summary>
    ///  Removes a shelter based on the shelter ID passed as an argument. Retrieves the shelter entity to make sure 
    ///  that the shelter belongs to the user ID which is also passed as an argument. After the deletion of a shelter
    ///  the user's role 'ShelterOwner' will be unassigned.
    /// </summary>
    /// <param name="id">The ID of the shelter resource to be deleted.</param>
    /// <param name="userId">The ID of the user requesting the deletion.</param>
    /// <returns>Returns a Task representing the asynchronous operation. No data is returned upon completion.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the retrieved shelter is null. The shelter could not be found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the User ID associated with the shelter does not match the user ID that was passed in.</exception>
    /// <remarks>
    /// This method will trigger cascade deletion of all pets associated with the shelter due to database configuration.
    /// However, adoption applications related to those pets will remain in the database to preserve historical data.
    /// The method performs ownership verification before deletion to ensure users can only delete their own shelters.
    /// </remarks>
    public async Task RemoveShelterAsync(int id, string userId)
    {
        logger.LogInformation("Starting deletion of shelter with ID: {ShelterId}. Deletion request made by user ID: {RequestingUserId}", id, userId);

        var shelter = await shelterRepository.FetchShelterByIdAsync(id);
        if (shelter == null)
        {
            logger.LogWarning("The shelter with ID: {ShelterId} could not be found", id);
            throw new KeyNotFoundException($"The shelter with ID: {id} could not be found.");
        }

        if (shelter.UserId != userId)
        {
            logger.LogWarning("Authorization failure: User {RequestingUserId} attempted to delete shelter {ShelterId} owned by user {UserId}", userId, id, shelter.UserId);
            throw new UnauthorizedAccessException("You do not have permission to delete this shelter.");
        }

        logger.LogDebug("Deleting shelter {ShelterId} with {PetCount} associated pets.", id, shelter.Pets.Count);

        await shelterRepository.DeleteShelterAsync(shelter);

        logger.LogDebug("Successfully deleted shelter with ID: {ShelterId} belonging to user {UserId}.", id, userId);

        await TryRemoveShelterOwnerRoleAsync(userId);
    }

    #region Helper Methods

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
            logger.LogWarning("Could not find user with ID: {UserId} to remove ShelterOwner role", userId);
            return;
        }

        const int maxRetries = 3;
        bool roleRemoved = false;
        int attempt = 0;

        while (!roleRemoved && attempt < maxRetries)
        {
            attempt++;
            logger.LogDebug("Attempt {Attempt} to remove 'ShelterOwner' role from user {UserId}", attempt, userId);

            var roleResult = await userManager.RemoveFromRoleAsync(user, "ShelterOwner");
            roleRemoved = roleResult.Succeeded;

            if (roleRemoved)
            {
                logger.LogDebug("Successfully removed 'ShelterOwner' role from user {UserId}", userId);
                return;
            }

            if (attempt < maxRetries)
            {
                int delayMilliseconds = 100 * (int)Math.Pow(2, attempt - 1);
                logger.LogWarning("Failed to remove ShelterOwner role. Retrying in {Delay}ms...", delayMilliseconds);
                await Task.Delay(delayMilliseconds);
            }
            else
            {
                logger.LogError("All attempts to remove ShelterOwner role from user {UserId} failed", userId);
            }
        }
    }

    /// <summary>
    /// Validates input parameters for shelter creation, ensuring all required data is present and properly formatted.
    /// This is a private helper method called by RegisterShelterAsync().
    /// </summary>
    /// <param name="userId">The string userId which needs to be checked for null and empty.</param>
    /// <param name="request">The request DTO that needs to be validated based on format of the Email provided, null and white space, Name.Length and Description.Length.</param>
    /// <exception cref="ArgumentException">Thrown when the userId is null or empty or when the request object is null.</exception>
    /// <exception cref="ValidationException">Thrown when any validation rule fails for Email format, Name (must not be empty and must be 3-50 characters), or Description (maximum 1000 characters if provided).</exception>
    private void ValidateRegisterShelterRequest(string userId, RegisterShelterRequest request)
    {
        logger.LogDebug("Validating shelter registration request for user {UserId}", userId);

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Shelter registration rejected: User ID is null or empty");
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        if (request == null)
        {
            logger.LogWarning("Shelter registration rejected: Request object is null");
            throw new ArgumentException("Request cannot be null.", nameof(request));
        }

        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            logger.LogWarning("Shelter registration rejected: Invalid email format");
            throw new ValidationException("Invalid email format.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            logger.LogWarning("Shelter registration rejected: Name is null or whitespace");
            throw new ValidationException("The shelter name cannot be null.");
        }

        if (request.Name.Length < 3)
        {
            logger.LogWarning(
                "Shelter registration rejected: Name too short (length: {NameLength})",
                request.Name.Length
            );
            throw new ValidationException("The shelter name must be at least 3 characters.");
        }

        if (request.Name.Length > 50)
        {
            logger.LogWarning(
                "Shelter registration rejected: Name too long (length: {NameLength})",
                request.Name.Length
            );
            throw new ValidationException("The shelter name cannot be more than 50 characters.");
        }

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
        {
            logger.LogWarning(
                "Shelter registration rejected: Description too long (length: {DescriptionLength})",
                request.Description.Length
            );
            throw new ValidationException(
                "The shelter description cannot be more than 1000 characters."
            );
        }
    }

    #endregion
}
