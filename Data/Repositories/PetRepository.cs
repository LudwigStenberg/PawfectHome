using Microsoft.EntityFrameworkCore;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext appDbContext;

    public PetRepository(AppDbContext dbContext)
    {
        appDbContext = dbContext;
    }

    public async Task<PetEntity> FetchPetAsync(int id)
    {
        return await appDbContext.Pets.FirstOrDefaultAsync(p => p.Id == id);
    }
}
