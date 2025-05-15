using Microsoft.EntityFrameworkCore;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext appDbContext;

    public PetRepository(AppDbContext dbContext)
    {
        appDbContext = dbContext;
    }

    public Task<PetEntity> CreatePetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<PetEntity> FetchPetAsync(int id)
    {
        var pet = await appDbContext
            .Pets.Include(p => p.Shelter)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pet;
    }

    public async Task<PetEntity> CreatePetAsync(PetEntity petEntity)
    {
        await appDbContext.Pets.AddAsync(petEntity);
        await appDbContext.SaveChangesAsync();
        return petEntity;
    }
}
