public class AdoptionRepository : IAdoptionRepository
{
    private readonly AppDbContext context;


    public AdoptionRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication)
    {
        context.AdoptionApplictions.Add(newAdoptionApplication);
        await context.SaveChangesAsync();
        return newAdoptionApplication;

    }
}