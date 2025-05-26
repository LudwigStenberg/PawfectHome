public interface IShelterService
{
    Task<(RegisterShelterResponse Shelter, bool AuthChanged)> RegisterShelterAsync(
        string userId,
        RegisterShelterRequest request
    );
    Task<ShelterDetailResponse> GetShelterAsync(int id);
    Task<ICollection<ShelterSummaryResponse>> GetAllSheltersAsync();
    Task<ShelterDetailResponse> UpdateShelterAsync(
        int id,
        string userId,
        ShelterUpdateRequest request
    );
    Task RemoveShelterAsync(int id, string userId);
}
