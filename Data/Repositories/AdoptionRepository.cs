using System.Data;
using Microsoft.EntityFrameworkCore;

public class AdoptionRepository : IAdoptionRepository
{
    private readonly AppDbContext context;

    public AdoptionRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<AdoptionApplicationEntity> CreateAdoptionApplicationAsync(
        AdoptionApplicationEntity newAdoptionApplication
    )
    {
        context.AdoptionApplications.Add(newAdoptionApplication);
        await context.SaveChangesAsync();
        return newAdoptionApplication;
    }

    public async Task<AdoptionApplicationEntity?> FetchAdoptionApplicationByIdAsync(int id)
    {
        return await context
            .AdoptionApplications.Include(a => a.User)
            .Include(a => a.Pet)
            .ThenInclude(p => p.Shelter)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AdoptionApplicationEntity>> FetchAllAdoptionsAsync(string userId)
    {
        return await context
            .AdoptionApplications.Include(a => a.User)
            .Include(a => a.Pet)
            .ThenInclude(p => p.Shelter)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AdoptionApplicationEntity>> FetchAllShelterAdoptionsAsync(
        string userId
    )
    {
        return await context
            .AdoptionApplications.Include(a => a.User)
            .Include(a => a.Pet)
            .ThenInclude(p => p.Shelter)
            .Where(a => a.Pet.Shelter.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }

    public async Task<AdoptionApplicationEntity> UpdateAdoptionStatusAsync(
        AdoptionApplicationEntity application
    )
    {
        context.AdoptionApplications.Update(application);

        await context.SaveChangesAsync();
        return application;
    }

    public async Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication)
    {
        context.AdoptionApplications.Remove(adoptionApplication);
        await context.SaveChangesAsync();
    }
}
