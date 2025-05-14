using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;
    private readonly IPetRepository petRepository;

    public PetService(AppDbContext appdbContext, IPetRepository petRepository)
    {
        dbContext = appdbContext;
        this.petRepository = petRepository;
    }

    public async Task<PetEntity> GetPetAsync(int id)
    {
        var pet = await dbContext.Pets.FirstOrDefaultAsync(p => p.Id == id);

        if (pet == null)
        {
            throw new KeyNotFoundException("Pet not found");
        }
        return pet;
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
