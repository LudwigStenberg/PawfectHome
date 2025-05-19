public class AdoptionRepository : IAdoptionRepository
{
    private readonly AppDbContext context;


    public AdoptionRepository(AppDbContext context)
    {
        this.context = context;
    }
    public async Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication)
    {
        context.AdoptionApplications.Add(newAdoptionApplication);
        await context.SaveChangesAsync();
        return newAdoptionApplication;
    }

    public async Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication)
    {
        throw new NotImplementedException();
    }
}