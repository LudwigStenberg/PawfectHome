public interface IAdoptionService
{
    public Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(RegisterAdoptionRequest request);
    Task DeleteAdoptionApplicationAsync(int id, string userId);
}