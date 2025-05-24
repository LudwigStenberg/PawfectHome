public interface IAdoptionRepository
{
    public Task<AdoptionApplicationEntity> CreateAdoptionAsync(
        AdoptionApplicationEntity newAdoptionApplication
    );
    public Task<AdoptionApplicationEntity> FetchAdoptionApplicationByIdAsync(int id);
    Task DeleteAdoptionApplicationAsync(AdoptionApplicationEntity adoptionApplication);
    public Task<AdoptionApplicationEntity> UpdateAdoptionStatusAsync(
        int id,
        AdoptionStatus updatedstatus,
        string userId
    );
}
