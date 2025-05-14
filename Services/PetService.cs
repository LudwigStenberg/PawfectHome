using Microsoft.EntityFrameworkCore;

public class PetService : IPetService
{
    private readonly AppDbContext dbContext;

    /// <summary>
    /// Initialize instance of the PetService class.
    /// </summary>
    /// <param name="appdbContext"> The database context for pet operations.</param>

    public PetService(AppDbContext appdbContext)
    {
        dbContext = appdbContext;
    }

    /// <summary>
    /// Retrieves a pet from the database by its ID, or throws an exception if not found.
    /// </summary>
    /// <param name="id">unique identifier of specific pet to be retrieved.</param>
    /// <returns>the task result contains the pet entity</returns>
    /// <exception cref="KeyNotFoundException"> Throw when no pet with the specified id is found.</exception>

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
