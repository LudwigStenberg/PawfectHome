public interface IAdoptionRepository
{
    Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication);
}