using Microsoft.EntityFrameworkCore;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext context;

    public PetRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<PetEntity> CreatePetAsync(PetEntity petEntity)
    {
        await context.Pets.AddAsync(petEntity);
        await context.SaveChangesAsync();
        return petEntity;
    }

    public async Task<PetEntity?> FetchPetAsync(int id)
    {
        return await context.Pets.Include(p => p.Shelter).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PetEntity>> FetchAllPetsAsync()
    {
        return await context.Pets.Include(p => p.Shelter).ToListAsync();
    }

    public async Task UpdatePetAsync(PetEntity existingPet)
    {
        context.Pets.Update(existingPet);
        await context.SaveChangesAsync();
    }

    public async Task DeletePetAsync(PetEntity petEntity)
    {
        context.Pets.Remove(petEntity);
        await context.SaveChangesAsync();
    }
}
