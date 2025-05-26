using System.ComponentModel.DataAnnotations;
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
    /// <exception cref="KeyNotFoundException">Thrown when the specified user or pet is not found</exception>
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

            throw new KeyNotFoundException($"No user found with ID {userId}.");

        }


        var petExists = await dbContext.Pets.AnyAsync(p => p.Id == request.PetId);
        if (!petExists)
        {
            throw new PetNotFoundException(request.PetId);
        }

        var adoptionApplicationEntity = new AdoptionApplicationEntity
        {
            UserId = userId,
            PetId = request.PetId,
            AdoptionStatus = AdoptionStatus.Pending,
        };


        var createdAdoptionApplication = await adoptionRepository.CreateAdoptionAsync(
            adoptionApplicationEntity
        );

        var response = AdoptionApplicationMapper.ToRegisterResponse(createdAdoptionApplication);

        logger.LogInformation(
            "Adoption application successfully registered for User: {UserId} and Pet: {PetId}.",
            createdAdoptionApplication.UserId,
            createdAdoptionApplication.PetId
        );

        return response;
    }

    /// <summary>
    /// Retrieves an adoption application based on its ID.
    /// </summary>
    /// <param name="id">The ID of the adoption application to retrieve</param>
    /// <param name="userId">The ID of the user requesting the adoption application</param>
    /// <returns>A response containing information about the found adoption application</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified adoption application is not found</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user doesn't own the adoption application</exception>
    public async Task<GetAdoptionApplicationResponse> GetAdoptionApplicationAsync(int id, string userId)
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
            throw new KeyNotFoundException($"No adoption application found with ID {id}.");
        }


        if (adoptionApplication.UserId != userId)
        {
            logger.LogWarning(
                "User {UserId} attempted to access adoption application {Id} belonging to user {OwnerId}",
                userId,
                id,
                adoptionApplication.UserId
            );
            throw new UnauthorizedAccessException("You can only access your own adoption applications.");

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
    public async Task<IEnumerable<GetAdoptionApplicationResponse>> GetAllAdoptionApplicationsAsync(string userId)
    {
        logger.LogInformation("Retrieving all adoption applications for user: {UserId}", userId);


        var adoptionApplications = await adoptionRepository.FetchAllAdoptionsAsync(userId);


        var responses = adoptionApplications.Select(application => new GetAdoptionApplicationResponse
        {
            Id = application.Id,
            CreatedDate = application.CreatedDate,
            AdoptionStatus = application.AdoptionStatus,
            UserId = application.UserId,
            PetId = application.PetId,
            PetName = application.Pet?.Name
        }).ToList();

        logger.LogInformation(
            "Successfully retrieved {Count} adoption applications for user: {UserId}",
            responses.Count,
            userId
        );

        return responses;
    }

    /// <summary>
    ///  Removes an adoption application based on the ID passed as an argument. Retrieves the adoption application entity to make sure 
    /// Updates the adoption status of an adoption application.
    /// </summary>
    /// <param name="id">The unique identifier of the adoption application to update.</param>
    /// <param name="request">The request containing the new adoption status.</param>
    /// <param name="userId">The ID of the user making the update request.</param>
    /// <returns>The updated adoption application entity.</returns>
    /// <exception cref="ArgumentException">Thrown when the request is null.</exception>
    public async Task<AdoptionApplicationEntity> UpdateAdoptionStatusAsync(
        int id,
        UpdateAdoptionStatusRequest request,
        string userId
    )
    {
        if (request != null)
        {
            return await adoptionRepository.UpdateAdoptionStatusAsync(
                id,
                request.AdoptionStatus,
                userId
            );
        }
        throw new ArgumentException();
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
                "The adoption application with ID {AdoptionApplicationId} could not be found", id);
            throw new AdoptionApplicationNotFoundException(id);
        }

        if (adoptionApplication.UserId != userId)
        {
            logger.LogWarning(
                "Authorization failure: User {RequestingUserId} attempted to delete adoption application {AdoptionApplicationId} owned by user with ID: '{UserId}'.",
                userId, id, adoptionApplication.UserId);
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
