public interface IShelterRepository
{
    Task<ShelterEntity> CreateShelterAsync(ShelterEntity newShelter);
    Task<ShelterEntity?> FetchShelterByIdAsync(int id);
    Task<ShelterEntity?> FetchShelterByUserIdAsync(string userId);
    Task<ICollection<ShelterWithPetCount>> FetchAllSheltersAsync();
    Task UpdateShelterAsync(ShelterEntity existingShelter);
    Task DeleteShelterAsync(ShelterEntity shelter);
    Task<bool> DoesShelterExistForUserAsync(string userId);
}
