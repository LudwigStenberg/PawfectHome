public interface IAdoptionRepository
{
    Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication);
    Task RemoveAdoptionApplicationAsync(int id, string userId);
}