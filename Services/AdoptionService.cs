using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
    /// <param name="request">
    /// The adoption application details to be registered, including user ID and pet ID.
    /// </param>
    /// <returns>
    /// A response containing the registered adoption application's details.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the specified user or pet is not found.
    /// </exception>
    /// <exception cref="ValidationFailedException">
    /// Thrown when the model validation fails.
    /// </exception>
    public async Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(RegisterAdoptionRequest request)
    {
        logger.LogInformation("Validating RegisterAdoptionRequest for registration");
        modelValidator.ValidateModel(request);

        var userExists = await dbContext.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            throw new KeyNotFoundException($"No user found with ID {request.UserId}.");
        }

        var petExists = await dbContext.Pets.AnyAsync(p => p.Id == request.PetId);
        if (!petExists)
        {
            throw new KeyNotFoundException($"No pet found with ID {request.PetId}.");
        }

        var adoptionApplicationEntity = new AdoptionApplicationEntity
        {
            UserId = request.UserId,
            PetId = request.PetId,
            AdoptionStatus = AdoptionStatus.Pending
        };

        var createdAdoptionApplication = await adoptionRepository.CreateAdoptionAsync(adoptionApplicationEntity);

        var response = new RegisterAdoptionResponse
        {
            Id = createdAdoptionApplication.Id,
            CreatedDate = createdAdoptionApplication.CreatedDate,
            AdoptionStatus = createdAdoptionApplication.AdoptionStatus,
            UserId = createdAdoptionApplication.UserId,
            PetId = createdAdoptionApplication.PetId
        };

        logger.LogInformation(
            "Adoption application successfully registered for User: {UserId} and Pet: {PetId}.",
            createdAdoptionApplication.UserId,
            createdAdoptionApplication.PetId
        );

        return response;
    }

    public async Task RemoveAdoptionApplicationAsync(int id, string userId)
    {
        // Retrieve the application (Eg: GET -- FetchAdoptionApplicationById())
        // Make sure it's not null : else, throw
        // Check that the User ID of the retrieved one matches the one in parameter
        // Make sure it's a match : else, throw
        // All good? call repository : DeleteAdoptionApplicationAsync()
    }
}