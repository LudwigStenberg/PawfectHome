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
}
