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
        UserManager<UserEntity> userManager)
    {
        this.logger = logger;
        this.shelterRepository = shelterRepository;
        this.userManager = userManager;
    }


    /// <summary>
    /// Registers a new shelter for a user in the system. Enforces the business rule that a user 
    /// can only have one shelter at a time. Additionally, assigns the user with a new role: "ShelterOwner" in a non-atomic operation.
    /// </summary>
    /// <param name="userId">The ID of the user that has requested the shelter registration.</param>
    /// <param name="request">The request DTO which contains properties for 'Name', 'Description' and 'Email'.</param>
    /// <returns>A RegisterShelterResponse DTO which contains the shelter's 'Id', 'Name', 'Description', 'Email' and the 'UserId' for the associated user.</returns>
    /// <exception cref="ArgumentException">Thrown when userId is null or empty, or when the request object is null.</exception>
    /// <exception cref="ValidationException">Thrown when a user who already has a shelter attempts to register another one. Each user can only have one shelter at a time.</exception>
    public async Task<RegisterShelterResponse> RegisterShelterAsync(string userId, RegisterShelterRequest request)
    {

        logger.LogInformation("Starting shelter registration for user {UserId}.", userId);

        ValidateRegisterShelterRequest(userId, request);

        bool existingShelter = await shelterRepository.DoesShelterExistForUserAsync(userId);
        if (existingShelter)
        {
            logger.LogWarning("User {UserId} attempted to register a second shelter.", userId);
            throw new ValidationException("User already has a shelter. Each user can only have one shelter registered at a time.");
        }

        var newShelter = new ShelterEntity
        {
            Name = request.Name,
            Description = request.Description ?? "No description",
            Email = request.Email,
            UserId = userId,
        };

        logger.LogInformation("Creating new shelter for user {UserId} with name {ShelterName}.", userId, newShelter.Name);

        var createdShelter = await shelterRepository.CreateShelterAsync(newShelter);

        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            logger.LogInformation("Assigning ShelterOwner role to user {UserId}.", userId);
            var roleResult = await userManager.AddToRoleAsync(user, "ShelterOwner");

            if (!roleResult.Succeeded)
            {
                var errorMessage = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                logger.LogWarning("Failed to assign ShelterOwner role to user {UserId}. Errors: {Errors}",
                    userId, errorMessage);
            }
        }
        else
        {
            logger.LogWarning("User {UserId} not found when trying to assign ShelterOwner role.", userId);
        }

        var response = new RegisterShelterResponse
        {
            Id = createdShelter.Id,
            Name = createdShelter.Name,
            Description = createdShelter.Description,
            Email = createdShelter.Email,
            UserId = createdShelter.UserId
        };

        logger.LogInformation("Successfully created shelter {ShelterId} for user {UserId}.", createdShelter.Id, userId);
        return response;
    }



    public async Task<ShelterResponse> GetShelterAsync(int id)
    {
        var shelter = await shelterRepository.FetchShelterByIdAsync(id);

        if (shelter is null)
        {
            throw new KeyNotFoundException($"Shelter with ID {id} could not be found.");
        }

        return new ShelterResponse()
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
                Species = pet.Species,

            }).ToList()
        };
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
            logger.LogWarning("Shelter registration rejected: Name too short (length: {NameLength})", request.Name.Length);
            throw new ValidationException("The shelter name must be at least 3 characters.");
        }

        if (request.Name.Length > 50)
        {
            logger.LogWarning("Shelter registration rejected: Name too long (length: {NameLength})", request.Name.Length);
            throw new ValidationException("The shelter name cannot be more than 50 characters.");
        }

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
        {
            logger.LogWarning("Shelter registration rejected: Description too long (length: {DescriptionLength})", request.Description.Length);
            throw new ValidationException("The shelter description cannot be more than 1000 characters.");
        }
    }
}