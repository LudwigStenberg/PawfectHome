using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly ILogger<PetService> logger;
    private readonly IPetRepository petRepository;

    public PetService(IPetRepository petRepository, ILogger<PetService> logger)
    {
        this.petRepository = petRepository;
        this.logger = logger;
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
            Age = pet.Age,
            Gender = pet.Gender,
            Species = pet.Species,
            Breed = pet.Breed,
            Description = pet.Description,
            ImageURL = pet.ImageURL,
            IsNeutured = pet.IsNeutured,
            HasPedigree = pet.HasPedigree,
            ShelterId = pet.ShelterId,
        };

        if (pet.Shelter != null)
        {
            response.shelter = new ShelterSummaryResponse
            {
                Id = pet.Shelter.Id,
                Name = pet.Shelter.Name,
                Description = pet.Shelter.Description,
                Email = pet.Shelter.Email,
            };
        }
        return response;
    }
}
