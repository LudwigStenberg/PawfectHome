public interface IAdoptionService
{
    public Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(
        RegisterAdoptionRequest request
    );

    public Task<GetAdoptionApplicationResponse> GetAdoptionApplicationAsync(GetAdoptionApplicationRequest request);
    Task RemoveAdoptionApplicationAsync(int id, string userId);

}
