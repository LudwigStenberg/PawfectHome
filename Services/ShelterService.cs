using System.ComponentModel.DataAnnotations;

public class ShelterService : IShelterService
{

    private readonly IShelterRepository shelterRepository;

    public ShelterService(IShelterRepository shelterRepository)
    {
        this.shelterRepository = shelterRepository;
    }

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


    // This is a helper method used within RegisterShelterAsync to make it more clean.
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
            throw new ValidationException("The shelter must be at least 3 characters long.");
        }

        if (request.Name.Length > 50)
        {
            throw new ValidationException("The shelter name cannot be more than 50 characters long.");
        }

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
        {
            throw new ValidationException("The shelter description cannot be more than 1000 characters long.");
        }
    }
}