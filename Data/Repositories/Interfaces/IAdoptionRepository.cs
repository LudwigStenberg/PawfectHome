public interface IAdoptionRepository
{
    Task<AdoptionApplicationEntity> CreateAdoptionApplicationAsync(
        AdoptionApplicationEntity newAdoptionApplication
    );
    Task<AdoptionApplicationEntity> FetchAdoptionApplicationByIdAsync(int id);
    Task<IEnumerable<AdoptionApplicationEntity>> FetchAllAdoptionsAsync(string userId);
    Task<IEnumerable<AdoptionApplicationEntity>> FetchAllShelterAdoptionsAsync(string userId);
    public Task<AdoptionApplicationEntity> UpdateAdoptionStatusAsync(
        AdoptionApplicationEntity application
    );
    Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication);
}
