public interface IAdoptionRepository
{
    Task<AdoptionApplicationEntity> CreateAdoptionAsync(AdoptionApplicationEntity newAdoptionApplication);
    Task<AdoptionApplicationEntity> FetchAdoptionApplicationByIdAsync(int id);
    Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication);
    Task<IEnumerable<AdoptionApplicationEntity>> FetchAllAdoptionsAsync(string userId);
    public Task<AdoptionApplicationEntity> UpdateAdoptionStatusAsync(
        int id,
        AdoptionStatus updatedstatus,
        string userId
    );
}
