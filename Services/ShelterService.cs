using System.ComponentModel.DataAnnotations;

public class ShelterService : IShelterService
{

    private readonly IShelterRepository shelterRepository;

    public ShelterService(IShelterRepository shelterRepository)
    {
        this.shelterRepository = shelterRepository;
    }


    /// <summary>
    /// Registers a new shelter for a user in the system. Enforces the business rule that a user 
    /// can only have one shelter at a time.
    /// </summary>
    /// <param name="userId">The ID of the user that has requested the shelter registration.</param>
    /// <param name="request">The request DTO which contains properties for 'Name', 'Description' and 'Email'.</param>
    /// <returns>A CreateShelterResponse DTO which contains the shelter's 'Id', 'Name', 'Description', 'Email' and the 'UserId' for the associated user.</returns>
    /// <exception cref="ArgumentException">Thrown when userId is null or empty, or when the request object is null.</exception>
    /// <exception cref="ValidationException">Thrown when a user who already has a shelter attempts to register another one. Each user can only have one shelter at a time.</exception>
    public async Task<CreateShelterResponse> RegisterShelterAsync(string userId, CreateShelterRequest request)
    {

        ValidateCreateShelterRequest(userId, request);

        bool existingShelter = await shelterRepository.DoesShelterExistForUserAsync(userId);
        if (existingShelter)
        {
            throw new ValidationException("User already has a shelter. Each user can only have one shelter registered at a time.");
        }

        var newShelter = new ShelterEntity
        {
            Name = request.Name,
            Description = request.Description ?? "No description",
            Email = request.Email,
            UserId = userId,
        };

        var createdShelter = await shelterRepository.CreateShelterAsync(newShelter);

        var response = new CreateShelterResponse
        {
            Id = createdShelter.Id,
            Name = createdShelter.Name,
            Description = createdShelter.Description,
            Email = createdShelter.Email,
            UserId = createdShelter.UserId
        };

        return response;
    }


    /// <summary>
    /// Validates input parameters for shelter creation, ensuring all required data is present and properly formatted.
    /// This is a private helper method called by RegisterShelterAsync().
    /// </summary>
    /// <param name="userId">The string userId which needs to be checked for null and empty.</param>
    /// <param name="request">The request DTO that needs to be validated based on format of the Email provided, null and white space, Name.Length and Description.Length.</param>
    /// <exception cref="ArgumentException">Thrown when the userId is null or empty or when the request object is null.</exception>
    /// <exception cref="ValidationException">Thrown when any validation rule fails for Email format, Name (must not be empty and must be 3-50 characters), or Description (maximum 1000 characters if provided).</exception>
    private void ValidateCreateShelterRequest(string userId, CreateShelterRequest request)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        if (request == null)
        {
            throw new ArgumentException("Request cannot be null.", nameof(request));
        }

        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            throw new ValidationException("Invalid email format.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("The shelter name cannot be null.");
        }

        if (request.Name.Length < 3)
        {
            throw new ValidationException("The shelter name must be at least 3 characters.");
        }

        if (request.Name.Length > 50)
        {
            throw new ValidationException("The shelter name cannot be more than 50 characters.");
        }

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
        {
            throw new ValidationException("The shelter description cannot be more than 1000 characters.");
        }
    }
}