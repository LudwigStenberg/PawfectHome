public interface IAdoptionRepository
{
    Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication);
    Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication);
}