

using System.ComponentModel.DataAnnotations;

public class ShelterService : IShelterService
{
    public Task<CreateShelterResponse> RegisterShelterAsync(string userId, CreateShelterRequest request)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        if (request == null)
        {
            throw new ArgumentException("Request cannot be null", nameof(request));
        }

        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            throw new ValidationException("Invalid email format");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            throw new ValidationException("The shelter name cannot be null");
        }

        if (request.Name.Length < 3)
        {
            throw new ValidationException("The shelter name must be at least 3 characters long");
        }

        if (request.Name.Length > 50)
        {
            throw new ValidationException("The shelter cannot be more than 50 characters long");
        }

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
        {
            throw new ValidationException("The shelter description cannot be more than 1000 characters long");
        }


    }
}