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


    /// <summary>
    /// Retrieves an adoption application based on its ID.
    /// </summary>
    /// <param name="request">
    /// The request containing the ID of the adoption application to retrieve.
    /// </param>
    /// <returns>
    /// A response containing information about the found adoption application.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the specified adoption application is not found.
    /// </exception>
    /// <exception cref="ValidationFailedException">
    /// Thrown when the model validation fails.
    /// </exception>
    public async Task<GetAdoptionApplicationResponse> GetAdoptionApplicationAsync(GetAdoptionApplicationRequest request)
    {
        logger.LogInformation("Validating GetAdoptionApplicationRequest for retrieval of adoption with ID: {Id}", request.Id);


        modelValidator.ValidateModel(request);


        var adoptionApplication = await adoptionRepository.FetchAdoptionApplicationByIdAsync(request.Id);


        if (adoptionApplication == null)
        {
            logger.LogWarning("No adoption application found with ID: {Id}", request.Id);
            throw new KeyNotFoundException($"No adoption application found with ID {request.Id}.");
        }


        var response = new GetAdoptionApplicationResponse
        {
            Id = adoptionApplication.Id,
            CreatedDate = adoptionApplication.CreatedDate,
            AdoptionStatus = adoptionApplication.AdoptionStatus,
            UserId = adoptionApplication.UserId,
            PetId = adoptionApplication.PetId,
            PetName = adoptionApplication.Pet?.Name,
        };

        logger.LogInformation(
            "Adoption application with ID: {Id} successfully retrieved for user ID: {UserId} and pet ID: {PetId}",
            adoptionApplication.Id,
            adoptionApplication.UserId,
            adoptionApplication.PetId
        );

        return response;
    }
}
