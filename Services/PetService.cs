using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;

    public PetService(AppDbContext appdbContext)
    {
        dbContext = appdbContext;
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
}
