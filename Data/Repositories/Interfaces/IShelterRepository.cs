
public interface IShelterRepository
{
    Task<ShelterEntity> CreateShelterAsync(ShelterEntity newShelter);
    Task<ShelterEntity?> FetchShelterByIdAsync(int id);
    Task<ShelterEntity?> FetchShelterByUserIdAsync(string userId);
    Task<bool> DoesShelterExistForUserAsync(string userId);
}