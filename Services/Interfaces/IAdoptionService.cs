public interface IAdoptionService
{
    Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(
        RegisterAdoptionRequest request,
        string userId
    );
    Task<GetAdoptionApplicationResponse> GetAdoptionApplicationAsync(int id, string userId);
    Task<IEnumerable<GetAdoptionApplicationResponse>> GetAllAdoptionApplicationsAsync(
        string userId
    );
    Task<IEnumerable<AdoptionApplicationShelterSummary>> GetAllShelterAdoptionApplicationsAsync(
        string userId
    );
    Task<AdoptionApplicationShelterSummary> UpdateAdoptionStatusAsync(
        int id,
        UpdateAdoptionStatusRequest updateAdoptionStatus,
        string userId
    );
    Task RemoveAdoptionApplicationAsync(int id, string userId);
}
