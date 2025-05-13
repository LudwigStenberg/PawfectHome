
public interface IShelterRepository
{
    Task<ShelterEntity> CreateShelterAsync(ShelterEntity newShelter);
    Task<ShelterEntity?> GetShelterByUserIdAsync(string userId);
    Task<bool> DoesShelterExistForUserAsync(string userId);
}