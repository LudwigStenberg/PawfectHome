using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;
    private readonly ILogger<IPetService> logger;
    private readonly IPetRepository petRepository;
    private readonly ModelValidator modelValidator;

    public PetService(
        IPetRepository petRepository,
        ILogger<IPetService> logger,
        AppDbContext appdbContext,
        ModelValidator modelValidator
    )
    {
        dbContext = appdbContext;
        this.petRepository = petRepository;
        this.modelValidator = modelValidator;
        this.logger = logger;
    }

    public async Task<IEnumerable<GetPetResponse>> GetAllPetsAsync()
    {
        logger.LogInformation("Retrieveing all pets");

        var pets = await petRepository.FetchAllPetsAsync();

        var responses = pets.Select(pet => new GetPetResponse
            {
                Id = pet.Id,
                Name = pet.Name,
                Birthdate = pet.Birthdate,
                Gender = pet.Gender,
                Species = pet.Species,
                Breed = pet.Breed,
                Description = pet.Description,
                ImageURL = pet.ImageURL,
                IsNeutured = pet.IsNeutered,
                HasPedigree = pet.HasPedigree,
                ShelterId = pet.ShelterId,
                Shelter =
                    pet.Shelter != null
                        ? new ShelterSummary
                        {
                            Id = pet.Shelter.Id,
                            Name = pet.Shelter.Name ?? "Unknown",
                            Description = pet.Shelter.Description ?? "No description",
                            Email = pet.Shelter.Email ?? "noemail@example.com",
                        }
                        : null,
            })
            .ToList();

        return responses;
    }

    /// <summary>
    /// Retrieves a pet from the database by its ID, or throws an exception if not found.
    /// </summary>
    /// <param name="id">unique identifier of specific pet to be retrieved.</param>
    /// <returns>the task result contains the pet entity</returns>
    /// <exception cref="KeyNotFoundException"> Throw when no pet with the specified id is found.</exception>

    public async Task<GetPetResponse> GetPetAsync(int id)
    {
        var pet = await petRepository.FetchPetAsync(id);

        if (pet == null)
        {
            logger.LogWarning("Pet with id {petId} was not found", id);
            throw new KeyNotFoundException("Pet not found");
        }

        var response = new GetPetResponse
        {
            Id = pet.Id,
            Name = pet.Name,
            Birthdate = pet.Birthdate,
            Gender = pet.Gender,
            Species = pet.Species,
            Breed = pet.Breed,
            Description = pet.Description,
            ImageURL = pet.ImageURL,
            IsNeutured = pet.IsNeutered,
            HasPedigree = pet.HasPedigree,
            ShelterId = pet.ShelterId,
        };
        if (pet.Shelter != null)
        {
            response.Shelter = new ShelterSummary
            {
                Id = pet.Shelter.Id,
                Name = pet.Shelter.Name ?? "Unknown",
                Description = pet.Shelter.Description ?? "No description",
                Email = pet.Shelter.Email ?? "noemail@example.com",
            };
        }
        return response;
    }

    public async Task<RegisterPetResponse> RegisterPetAsync(RegisterPetRequest request)
    {
        logger.LogInformation("Validatin RegisterPetRequest for registration");
        modelValidator.ValidateModel(request);

        var shelterExists = await dbContext.Shelters.AnyAsync(s => s.Id == request.ShelterId);
        if (!shelterExists)
            throw new KeyNotFoundException($"No shelter found with ID {request.ShelterId}.");

        var petEntity = new PetEntity
        {
            Name = request.Name,
            Birthdate = request.Birthdate,
            Gender = request.Gender,
            Species = request.Species,
            Breed = request.Breed,
            Description = request.Description,
            ImageURL = request.ImageURL,
            IsNeutered = request.IsNeutured,
            HasPedigree = request.HasPedigree,
            ShelterId = request.ShelterId,
        };

        var result = await petRepository.CreatePetAsync(petEntity);

        var response = new RegisterPetResponse
        {
            Id = result.Id,
            Name = result.Name,
            Birthdate = result.Birthdate,
            Gender = result.Gender,
            Species = result.Species,
            Breed = result.Breed,
            Description = result.Description,
            ImageURL = result.ImageURL,
            IsNeutered = result.IsNeutered,
            HasPedigree = result.HasPedigree,
            ShelterId = result.ShelterId,
            CreatedAt = DateTime.UtcNow,
        };

        logger.LogInformation(
            "Pet successfully registered: {Name} for ShelterId: {ShelterId}.",
            result.Name,
            result.ShelterId
        );

        return response;
    }
}
