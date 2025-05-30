using Microsoft.EntityFrameworkCore;

public class AdoptionService : IAdoptionService
{
    private readonly AppDbContext dbContext;
    private readonly ILogger<IAdoptionService> logger;
    private readonly IAdoptionRepository adoptionRepository;
    private readonly ModelValidator modelValidator;

    public AdoptionService(
        AppDbContext appdbContext,
        IAdoptionRepository adoptionRepository,
        ILogger<IAdoptionService> logger,
        ModelValidator modelValidator
    )
    {
        this.dbContext = appdbContext;
        this.adoptionRepository = adoptionRepository;
        this.logger = logger;
        this.modelValidator = modelValidator;
    }

    /// <summary>
    /// Registers a new adoption application in the database.
    /// </summary>
    /// <param name="request">The adoption application details to be registered</param>
    /// <param name="userId">The ID of the user creating the adoption application</param>
    /// <returns>A response containing the registered adoption application's details</returns>
    /// <exception cref="UserNotFoundException">Thrown when the specified user is not found</exception>
    /// <exception cref="PetNotFoundException">Thrown when the specified pet is not found</exception>
    /// <exception cref="ValidationFailedException">Thrown when the model validation fails</exception>

    public async Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(
        RegisterAdoptionRequest request,
        string userId
    )
    {
        logger.LogInformation("Validating RegisterAdoptionRequest for registration");
        modelValidator.ValidateModel(request);

        var userExists = await dbContext.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            logger.LogWarning(
                "User not found while creating an adoption application for UserId: {UserId}.",
                userId
            );
            throw new UserNotFoundException(userId);
        }

        var petExists = await dbContext.Pets.AnyAsync(p => p.Id == request.PetId);
        if (!petExists)
        {
            logger.LogWarning(
                "Pet not found while creating an adoption application for UserId: {UserId}.",
                userId
            );
            throw new PetNotFoundException(request.PetId);
        }

        var adoptionApplication = AdoptionApplicationMapper.ToEntity(request, userId);

        var createdAdoptionApplication = await adoptionRepository.CreateAdoptionApplicationAsync(
            adoptionApplication
        );

        logger.LogInformation(
            "Adoption application with ID {Id} successfully created for UserId: {UserId} and Pet: {PetId}.",
            createdAdoptionApplication.Id,
            userId,
            createdAdoptionApplication.PetId
        );

        var response = AdoptionApplicationMapper.ToRegisterResponse(createdAdoptionApplication);

        return response;
    }

    /// <summary>
    /// Retrieves an adoption application based on its ID.
    /// </summary>
    /// <param name="id">The ID of the adoption application to retrieve</param>
    /// <param name="userId">The ID of the user requesting the adoption application</param>
    /// <returns>A response containing information about the found adoption application</returns>
    /// <exception cref="AdoptionApplicationNotFoundException">Thrown when the specified adoption application is not found</exception>
    /// <exception cref="AdoptionApplicationOwnershipException">Thrown when the user doesn't own the adoption application</exception>
    public async Task<GetAdoptionApplicationResponse> GetAdoptionApplicationAsync(
        int id,
        string userId
    )
    {
        logger.LogInformation(
            "Retrieving adoption application with ID: {Id} for user: {UserId}",
            id,
            userId
        );

        var adoptionApplication = await adoptionRepository.FetchAdoptionApplicationByIdAsync(id);

        if (adoptionApplication == null)
        {
            logger.LogWarning("No adoption application found with ID: {Id}", id);
            throw new AdoptionApplicationNotFoundException(id);
        }

        if (adoptionApplication.UserId != userId)
        {
            logger.LogWarning(
                "User {UserId} attempted to access adoption application {Id} belonging to user {OwnerId}",
                userId,
                id,
                adoptionApplication.UserId
            );
            throw new AdoptionApplicationOwnershipException(id, userId);
        }

        var response = AdoptionApplicationMapper.ToGetResponse(adoptionApplication);

        logger.LogInformation(
            "Adoption application with ID: {Id} successfully retrieved for user: {UserId}",
            id,
            userId
        );

        return response;
    }

    /// <summary>
    /// Retrieves all adoption applications belonging to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose adoption applications should be retrieved</param>
    /// <returns>A collection of adoption applications belonging to the specified user</returns>
    public async Task<IEnumerable<GetAdoptionApplicationResponse>> GetAllAdoptionApplicationsAsync(
        string userId
    )
    {
        logger.LogInformation(
            "Retrieving all adoption applications for shelter owner: {UserId}",
            userId
        );

        var adoptionApplications = await adoptionRepository.FetchAllAdoptionsAsync(userId);

        var response = adoptionApplications
            .Select(AdoptionApplicationMapper.ToGetResponse)
            .ToList();

        logger.LogInformation(
            "Successfully retrieved {Count} adoption applications for user {UserId}",
            response.Count(),
            userId
        );

        return response;
    }

    /// <summary>
    /// Retrieves all adoption applications for pets belonging to a shelter owned by the specified user.
    /// </summary>
    /// <param name="userId">The ID of the shelter owner.</param>
    /// <returns>An enumerable collection of <see cref="AdoptionApplicationShelterSummary"/> containing application details.</returns>
    /// <exception cref="InvalidOperationException" .</<exception>

    public async Task<
        IEnumerable<AdoptionApplicationShelterSummary>
    > GetAllShelterAdoptionApplicationsAsync(string userId)
    {
        logger.LogInformation(
            "Retrieving all adoption applications for shelter owned by user: {UserId}",
            userId
        );

        var shelterAdoptionApplications = await adoptionRepository.FetchAllShelterAdoptionsAsync(
            userId
        );
        if (!shelterAdoptionApplications.Any())
        {
            logger.LogWarning(
                "No adoption applications found for shelter owned by user: {UserId}",
                userId
            );
            throw new InvalidOperationException("No adoption applications found for this shelter");
        }
        var response = shelterAdoptionApplications
            .Select(AdoptionApplicationMapper.ToUpdateResponse)
            .ToList();
        logger.LogInformation(
            "Successfully retrieved {Count} adoption applications for shelter owned by user {UserId}",
            response.Count,
            userId
        );

        return response;
    }

    /// <summary>
    /// Updates the adoption status of an adoption application.
    /// </summary>
    /// <param name="id">The unique identifier of the adoption application to update.</param>
    /// <param name="request">The request containing the new adoption status.</param>
    /// <param name="userId">The ID of the user making the update request.</param>
    /// <returns>A summary of the updated adoption application</returns>
    /// <exception cref="ArgumentException">Thrown when any adoption application is not found or the user lacks permission.</exception>
    /// <exception cref="KeyNotFoundException" Thrown when the adoption application is not found.</<exception>

    public async Task<AdoptionApplicationShelterSummary> UpdateAdoptionStatusAsync(
        int id,
        UpdateAdoptionStatusRequest request,
        string userId
    )
    {
        var shelterApplications = await GetAllShelterAdoptionApplicationsAsync(userId);
        if (shelterApplications == null)
        {
            logger.LogWarning(
                "Application status update failed - no applications found related to shelter. RequestedBy: {userId}",
                userId
            );
            throw new ArgumentException($"No applications found related to shelter");
        }
        var applicationExists = shelterApplications.Any(a => a.Id == id);

        if (!applicationExists)
        {
            logger.LogWarning(
                "Application status update failed - application not found. ApplicationId: {id}. Requested by: {userId}",
                id,
                userId
            );
            throw new KeyNotFoundException($"No application with ID {id}");
        }

        var updatedApplication = await adoptionRepository.UpdateAdoptionStatusAsync(
            id,
            request.AdoptionStatus,
            userId
        );

        var response = AdoptionApplicationMapper.ToUpdateResponse(updatedApplication!);

        return response;
    }

    /// <summary>
    ///  Removes an adoption application based on the ID passed as an argument. Retrieves the adoption application entity to make sure
    ///  that it belongs to the user ID which is also passed as an argument.
    /// </summary>
    /// <param name="id">The ID of the adoption application to be removed.</param>
    /// <param name="userId">The ID of the user requesting the removal of the adoption application.</param>
    /// <returns>Returns a Task representing the asynchronous operation. No data is returned upon completion.</returns>
    /// <exception cref="AdoptionApplicationNotFoundException">Thrown when retrieved adoption application is null. The resource could not be found.</exception>
    /// <exception cref="AdoptionApplicationOwnershipException">Thrown when the retrieved adoption application's User ID does not match the User ID provided by the caller.</exception>
    public async Task RemoveAdoptionApplicationAsync(int id, string userId)
    {
        logger.LogInformation(
            "Starting deletion of adoption application with ID: {AdoptionApplicationId}. Deletion request made by user ID: {RequestingUserId}",
            id,
            userId
        );

        var adoptionApplication = await adoptionRepository.FetchAdoptionApplicationByIdAsync(id);
        if (adoptionApplication == null)
        {
            logger.LogWarning(
                "The adoption application with ID {AdoptionApplicationId} could not be found",
                id
            );
            throw new AdoptionApplicationNotFoundException(id);
        }

        if (adoptionApplication.UserId != userId)
        {
            logger.LogWarning(
                "Authorization failure: User {RequestingUserId} attempted to delete adoption application {AdoptionApplicationId} owned by user with ID: '{UserId}'.",
                userId,
                id,
                adoptionApplication.UserId
            );
            throw new AdoptionApplicationOwnershipException(id, userId);
        }

        await adoptionRepository.DeleteAdoptionApplicationAsync(adoptionApplication);

        logger.LogDebug(
            "Successfully deleted adoption application with ID: {AdoptionApplicationId} for user with ID: {UserId}",
            id,
            userId
        );
    }
}
