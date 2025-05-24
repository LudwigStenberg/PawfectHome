public interface IAdoptionService
{
    Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(RegisterAdoptionRequest request, string userId);
    Task<GetAdoptionApplicationResponse> GetAdoptionApplicationAsync(int id, string userId);

    Task RemoveAdoptionApplicationAsync(int id, string userId);

    Task<AdoptionApplicationEntity> UpdateAdoptionStatusAsync(
        int id,
        UpdateAdoptionStatusRequest updateAdoptionStatus,
        string userId
    );
}
