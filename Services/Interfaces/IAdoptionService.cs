public interface IAdoptionService
{
    public Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(RegisterAdoptionRequest request);
    Task RemoveAdoptionApplicationAsync(int id, string userId);
}