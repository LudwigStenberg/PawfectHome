using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;
    private readonly IPetRepository petRepository;
    private readonly ModelValidator modelValidator;
    private readonly ILogger<IPetService> logger;

    public PetService(
        AppDbContext appdbContext,
        IPetRepository petRepository,
        ModelValidator modelValidator,
        ILogger<IPetService> logger
    )
    {
        dbContext = appdbContext;
        this.petRepository = petRepository;
        this.modelValidator = modelValidator;
        this.logger = logger;
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
        logger.LogInformation("Validating RegisterPetRequest for registration");
        modelValidator.ValidateModel(request);

        var shelterExists = await dbContext.Shelters.AnyAsync(s => s.Id == request.ShelterId);
        if (!shelterExists)
        {
            throw new KeyNotFoundException($"No shelter found with ID {request.ShelterId}.");
        }

        if (!DateTime.TryParseExact(request.Birthdate, "yyyy-MM-dd", null, DateTimeStyles.AssumeUniversal, out DateTime parsedBirthdate))
        {
        
            var errors = new List<ValidationResult>
            {
                new ValidationResult("Invalid birthdate format. Please use 'yyyy-MM-dd'.", new[] { "Birthdate" })
            };

            throw ValidationFailedException.FromValidationResults(errors);
        }
        DateTime utcBirthdate =
            parsedBirthdate.Kind == DateTimeKind.Utc
                ? parsedBirthdate
                : parsedBirthdate.ToUniversalTime();

        var petEntity = new PetEntity
        {
            Name = request.Name,
            Birthdate = utcBirthdate,
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
