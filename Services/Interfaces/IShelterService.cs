
public interface IShelterService
{
    Task<RegisterShelterDetailResponse> RegisterShelterAsync(string userId, RegisterShelterRequest request);
    Task<ShelterDetailResponse> GetShelterAsync(int id);
    Task<ICollection<ShelterSummaryResponse>> GetAllSheltersAsync();
    Task<ShelterDetailResponse> UpdateShelterAsync(string userId, int id, ShelterUpdateRequest request);
}