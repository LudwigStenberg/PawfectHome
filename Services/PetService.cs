using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;
    private readonly ILogger<PetService> logger;
    private readonly IPetRepository petRepository;

    public PetService(
        IPetRepository petRepository,
        ILogger<PetService> logger,
        AppDbContext appdbContext
    )
    {
        dbContext = appdbContext;
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
        var shelterExists = await dbContext.Shelters.AnyAsync(s => s.Id == request.ShelterId);
        if (!shelterExists)
            throw new KeyNotFoundException($"No shelter found with ID {request.ShelterId}.");

        var petEntity = new PetEntity
        {
            Name = request.Name,
            Age = request.Age,
            Gender = request.Gender,
            Species = request.Species,
            Breed = request.Breed,
            Description = request.Description,
            ImageURL = request.ImageURL,
            IsNeutured = request.IsNeutured,
            HasPedigree = request.HasPedigree,
            ShelterId = request.ShelterId,
        };

        var createdPet = await petRepository.CreatePetAsync(petEntity);

        var response = new RegisterPetResponse
        {
            Id = createdPet.Id,
            Name = createdPet.Name,
            Birthdate = createdPet.Birthdate,
            Gender = createdPet.Gender,
            Species = createdPet.Species,
            Breed = createdPet.Breed,
            Description = createdPet.Description,
            ImageURL = createdPet.ImageURL,
            IsNeutered = createdPet.IsNeutered,
            HasPedigree = createdPet.HasPedigree,
            ShelterId = createdPet.ShelterId,
            CreatedAt = DateTime.UtcNow,
        };

        return response;
    }
}
