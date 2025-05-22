using Microsoft.EntityFrameworkCore;

public class AdoptionRepository : IAdoptionRepository
{
    private readonly AppDbContext context;

    public AdoptionRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<AdoptionApplicationEntity> CreateAdoptionAsync(
        AdoptionApplicationEntity newAdoptionApplication
    )
    {
        context.AdoptionApplications.Add(newAdoptionApplication);
        await context.SaveChangesAsync();
        return newAdoptionApplication;
    }

    public async Task<AdoptionApplicationEntity> FetchAdoptionApplicationByIdAsync(int id)
    {
        return await context
            .AdoptionApplications.Include(a => a.User)
            .Include(a => a.Pet)
            .ThenInclude(p => p.Shelter)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication)
    {
        context.AdoptionApplications.Remove(adoptionApplication);
        await context.SaveChangesAsync();
    }
}
