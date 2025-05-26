using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;
    private readonly ILogger<IPetService> logger;
    private readonly IPetRepository petRepository;
    private readonly ModelValidator modelValidator;
    private readonly IShelterRepository shelterRepository;

    public PetService(
        IPetRepository petRepository,
        IShelterRepository shelterRepository,
        ILogger<IPetService> logger,
        AppDbContext appdbContext,
        ModelValidator modelValidator
    )
    {
        dbContext = appdbContext;
        this.petRepository = petRepository;
        this.modelValidator = modelValidator;
        this.logger = logger;
        this.shelterRepository = shelterRepository;
    }

    /// <summary>
    /// Retrieves all pets from database
    /// </summary>
    /// <returns> A collection of pets</returns>
    public async Task<IEnumerable<GetPetResponse>> GetAllPetsAsync()
    {
        logger.LogInformation("Retrieveing all pets");

        var pets = await petRepository.FetchAllPetsAsync();

        var responses = pets.Select(PetMapper.ToGetResponse).ToList();

        return responses;
    }

    /// <summary>
    /// Retrieves a pet from the database by its ID, or throws an exception if not found.
    /// </summary>
    /// <param name="id">unique identifier of specific pet to be retrieved.</param>
    /// <returns>the task result contains the pet entity</returns>
    /// <exception cref="PetNotFoundException"> Throw when no pet with the specified id is found.</exception>

    public async Task<GetPetResponse> GetPetAsync(int id)
    {
        var pet = await petRepository.FetchPetAsync(id);

        if (pet == null)
        {
            logger.LogWarning("Pet with id {petId} was not found", id);
            throw new PetNotFoundException(id);
        }

        var response = PetMapper.ToGetResponse(pet);

        return response;
    }

    /// <summary>
    /// Registers a new pet for the shelter associated with the requesting user.
    /// </summary>
    /// <param name="request">Details about the pet to register.</param>
    /// <param name="userId">The user ID of the shelter owner submitting the request.</param>
    /// <returns>The registered pet’s details.</returns>
    /// <exception cref="ShelterNotFoundException">
    /// Thrown if the specified shelter does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the user is not authorized to register a pet for the specified shelter.
    /// </exception>
    /// <exception cref="ValidationFailedException">
    /// Thrown if the request data is invalid or the birthdate format is incorrect.
    /// </exception>
    public async Task<RegisterPetResponse> RegisterPetAsync(
        RegisterPetRequest request,
        string userId
    )
    {
        logger.LogInformation("Validating RegisterPetRequest for registration");

        try
        {
            modelValidator.ValidateModel(request);
        }
        catch (ValidationFailedException ex)
        {
            logger.LogWarning(
                $"Pet registration failed when validating - {ex.Errors} RequestedBy: {userId}."
            );
            throw;
        }

        if (
            !TryParseAndValidateBirthdate(
                request.Birthdate,
                out DateTime parsedBirthdate,
                out List<ValidationResult> errors
            )
        )
        {
            logger.LogWarning(
                "Pet registration failed - invalid birthdate: {Birthdate}. {@Errors} RequestedBy: {UserId}.",
                request.Birthdate,
                errors,
                userId
            );
            throw ValidationFailedException.FromValidationResults(errors);
        }

        var shelter = await shelterRepository.FetchShelterByIdAsync(request.ShelterId);

        if (shelter == null)
        {
            logger.LogWarning(
                "Pet registration failed - The shelter with ID: {ShelterId} could not be found. RequestedBy; {UserId}",
                request.ShelterId,
                userId
            );
            throw new ShelterNotFoundException(request.ShelterId);
        }

        if (userId != shelter.UserId)
        {
            logger.LogWarning(
                "Pet registration failed - Unauthorized access. RequestedBy: {UserId}, "
                    + "OwnerUserId: {OwnerUserId}. ShelterId: {ShelterId}",
                userId,
                shelter.UserId,
                shelter.Id
            );
            throw new UnauthorizedAccessException("You are not authorized to register this pet.");
        }

        var petEntity = PetMapper.ToEntity(request, parsedBirthdate);

        var result = await petRepository.CreatePetAsync(petEntity);

        var response = PetMapper.ToRegisterResponse(result);

        logger.LogInformation(
            "Pet successfully registered. PetId: {PetId}, UserId: {UserId}, ShelterId: {ShelterId}",
            result.Id,
            userId,
            result.ShelterId
        );

        return response;
    }

    /// <summary>
    /// Removes a pet from the database.
    /// </summary>
    /// <param name="id">The id of the pet to be removed.</param>
    /// <exception cref="ValidationFailedException">Thrown when id is a non-positive integer.</exception>
    /// <exception cref="PetNotFoundException">Thrown when the specified pet is not found.</exception>
    /// <exception cref="ShelterNotFoundException">Thrown when the specified pet is not found.</exception>
    public async Task RemovePetAsync(int id, string userId)
    {
        logger.LogInformation(
            "Starting pet deletion operation. PetId: {PetId}, UserId: {UserId}",
            id,
            userId
        );

        if (id <= 0)
        {
            logger.LogWarning(
                "Pet deletion failed - Invalid PetId: {PetId}. Pet ID must be a positive number. RequestedBy: {UserId}",
                id,
                userId
            );
            throw modelValidator.CreateValidationFailure(
                "Invalid ID. The Pet ID must be a positive number.",
                "PetId"
            );
        }

        var pet = await petRepository.FetchPetAsync(id);

        if (pet == null)
        {
            logger.LogWarning(
                "Pet deletion failed - Pet could not be found. PetId: {PetId}. RequestedBy: {UserId}",
                id,
                userId
            );
            throw new PetNotFoundException(id);
        }

        var shelter = await shelterRepository.FetchShelterByUserIdAsync(userId);

        if (shelter == null)
        {
            logger.LogWarning(
                "Pet deletion failed - No shelter registered for UserId: {UserId} ",
                userId
            );
            throw new KeyNotFoundException($"No matching shelter found.");
        }

        if (shelter.Id != pet.ShelterId)
        {
            logger.LogWarning(
                "Pet deletion failed - Unauthorized access. PetId: {PetId}, RequestedBy: {UserId}, ",
                id,
                userId
            );
            throw new UnauthorizedAccessException("You are not authorized to delete this pet.");
        }

        await petRepository.DeletePetAsync(pet);

        logger.LogInformation(
            "Pet successfully deleted. PetId: {PetId}, ShelterId: {ShelterId} UserId: {UserId}",
            id,
            shelter.Id,
            userId
        );
    }

    /// <summary>
    /// Updates the details of a pet if it belongs to the user's shelter.
    /// </summary>
    /// <param name="petId">The ID of the pet to update. Must be a positive number.</param>
    /// <param name="userId">The ID of the shelter representative making the request.</param>
    /// <param name="request">The fields to update for the pet.</param>
    /// <returns>The updated pet’s details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the pet or the user's shelter cannot be found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if the pet does not belong to the user's shelter.</exception>
    /// <exception cref="ValidationFailedException">Thrown if any update field is invalid (e.g., name length, birthdate format, invalid enum values).</exception>
    public async Task<UpdatePetResponse> UpdatePetAsync(
        int petId,
        string userId,
        UpdatePetRequest request
    )
    {
        logger.LogInformation(
            "Starting pet update operation. PetId: {PetId}, UserId: {UserId}",
            petId,
            userId
        );

        if (petId <= 0)
        {
            logger.LogWarning(
                "Pet update failed - Invalid PetId: {PetId}. Pet ID must be a positive number. RequestedBy: {UserId}",
                petId,
                userId
            );

            throw modelValidator.CreateValidationFailure(
                "Invalid ID. The Pet ID must be a positive number.",
                "PetId"
            );
        }

        var existingPet = await petRepository.FetchPetAsync(petId);

        if (existingPet == null)
        {
            logger.LogWarning(
                "Pet update failed - Pet could not be found. PetId: {PetId}. RequestedBy: {UserId}",
                petId,
                userId
            );
            throw new PetNotFoundException(petId);
        }

        var shelter = await shelterRepository.FetchShelterByUserIdAsync(userId);

        if (shelter == null)
        {
            logger.LogWarning(
                "Pet deletion failed - No shelter registered for UserId: {UserId} ",
                userId
            );
            throw new KeyNotFoundException($"No matching shelter found.");
        }

        if (shelter.Id != existingPet.ShelterId)
        {
            logger.LogWarning(
                "Pet update failed - Unauthorized access. PetId: {PetId}, RequestedBy: {UserId}, ",
                petId,
                userId
            );
            throw new UnauthorizedAccessException("You are not authorized to update this pet.");
        }

        if (request.Name != null)
        {
            if (request.Name.Length < 3 || request.Name.Length > 50)
            {
                logger.LogWarning(
                    "Pet update failed - invalid name: {Name}. The pet name must be between 3 and 50 characters. RequestedBy: {UserId}.",
                    request.Name,
                    userId
                );
                throw modelValidator.CreateValidationFailure(
                    "Invalid name. The pet name must be between 3 and 50 characters.",
                    "Name"
                );
            }
            existingPet.Name = request.Name;
        }

        if (request.Birthdate != null)
        {
            if (
                !TryParseAndValidateBirthdate(
                    request.Birthdate,
                    out DateTime birthdate,
                    out List<ValidationResult> errors
                )
            )
            {
                logger.LogWarning(
                    "Pet update failed - invalid birthdate: {Birthdate}. {@Errors} RequestedBy: {UserId}.",
                    request.Birthdate,
                    errors,
                    userId
                );

                throw ValidationFailedException.FromValidationResults(errors);
            }
            existingPet.Birthdate = birthdate;
        }

        if (request.Gender.HasValue)
        {
            if (!Enum.IsDefined(typeof(Gender), request.Gender))
            {
                logger.LogWarning(
                    "Pet update failed - invalid gender: {gender}. Invalid gender. The pet gender must be a valid option RequestedBy: {UserId}.",
                    request.Gender,
                    userId
                );

                throw modelValidator.CreateValidationFailure(
                    "Invalid gender. The pet gender must be a valid option",
                    "Name"
                );
            }
            existingPet.Gender = request.Gender.Value;
        }
        if (request.Species.HasValue)
        {
            if (!Enum.IsDefined(typeof(Species), request.Species))
            {
                logger.LogWarning(
                    "Pet update failed - invalid species: {Species}. Invalid species. The pet species must be a valid option. RequestedBy: {UserId}.",
                    request.Species,
                    userId
                );

                throw modelValidator.CreateValidationFailure(
                    "Invalid species. The pet species must be a valid option",
                    "Name"
                );
            }
            existingPet.Species = request.Species.Value;
        }

        if (request.Breed != null)
            existingPet.Breed = request.Breed;

        if (request.Description != null)
        {
            if (request.Description.Length > 1000)
            {
                logger.LogWarning(
                    "Pet update failed - invalid description: {Description}. The pet description cannot be more than 1000 characters. RequestedBy: {UserId}.",
                    request.Description,
                    userId
                );

                throw modelValidator.CreateValidationFailure(
                    "Invalid description. The pet description cannot be more than 1000 characters.",
                    "Description"
                );
            }
            existingPet.Description = request.Description;
        }

        if (request.ImageURL != null)
            existingPet.ImageURL = request.ImageURL;

        if (request.IsNeutered.HasValue)
            existingPet.IsNeutered = request.IsNeutered.Value;

        if (request.HasPedigree.HasValue)
            existingPet.HasPedigree = request.HasPedigree.Value;

        await petRepository.UpdatePetAsync(existingPet);

        var response = PetMapper.ToUpdateResponse(existingPet);

        logger.LogInformation(
            "Pet successfully updated. PetId: {PetId}, UserId: {UserId}, ShelterId: {ShelterId}",
            existingPet.Id,
            userId,
            existingPet.ShelterId
        );

        return response;
    }

    #region Helper Methods
    /// <summary>
    /// Parses and validates a pet's birthdate string. Ensures format and logical constraints like not being in the future.
    /// </summary>
    /// <param name="input">The birthdate as a string in "yyyy-MM-dd" format.</param>
    /// <param name="birthdate">The parsed and UTC-adjusted birthdate, if valid.</param>
    /// <param name="errors">A list of validation errors if parsing or validation fails.</param>
    /// <returns>True if the birthdate is valid; otherwise, false.</returns>
    private static bool TryParseAndValidateBirthdate(
        string input,
        out DateTime birthdate,
        out List<ValidationResult> errors
    )
    {
        errors = [];
        if (
            !DateTime.TryParseExact(
                input,
                "yyyy-MM-dd",
                null,
                DateTimeStyles.AssumeUniversal,
                out birthdate
            )
        )
        {
            errors.Add(
                new ValidationResult(
                    "Invalid birthdate format. Please use 'yyyy-MM-dd'.",
                    new[] { "Birthdate" }
                )
            );
            return false;
        }

        if (birthdate > DateTime.UtcNow)
        {
            errors.Add(
                new ValidationResult(
                    "Invalid birthdate - Birthdate cannot be in the future.",
                    new[] { "Birthdate" }
                )
            );
            return false;
        }
        birthdate = birthdate.Kind == DateTimeKind.Utc ? birthdate : birthdate.ToUniversalTime();

        return true;
    }

    #endregion
}
