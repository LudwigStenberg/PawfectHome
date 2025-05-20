public interface IAdoptionRepository
{
    public Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication);

    public Task<AdoptionApplicationEntity> FetchAdoptionApplicationByIdAsync(int id);
}