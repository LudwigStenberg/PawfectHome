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

    public async Task<ShelterEntity?> GetShelterByUserIdAsync(string userId)
    {
        return await context.Shelters
            .Include(s => s.Pets)
            .SingleOrDefaultAsync(s => s.UserId == userId);
    }
}