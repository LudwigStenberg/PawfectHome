using Microsoft.EntityFrameworkCore;

public class ShelterRepository : IShelterRepository
{
    private readonly AppDbContext context;

    public ShelterRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<ShelterEntity> CreateShelterAsync(ShelterEntity newShelter)
    {
        context.Shelters.Add(newShelter);
        await context.SaveChangesAsync();
        return newShelter;
    }

    public async Task<ShelterEntity?> FetchShelterByIdAsync(int id)
    {
        return await context.Shelters
            .Include(s => s.Pets)
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ShelterEntity?> FetchShelterByUserIdAsync(string userId)
    {
        return await context.Shelters
            .SingleOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<ICollection<ShelterWithPetCountDto>> FetchAllSheltersAsync()
    {
        return await context.Shelters
            .Select(s => new ShelterWithPetCountDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Email = s.Email,
                PetCount = s.Pets.Count

            }).ToListAsync();
    }

    public async Task<bool> DoesShelterExistForUserAsync(string userId)
    {
        return await context.Shelters.AnyAsync(s => s.UserId == userId);
    }

}