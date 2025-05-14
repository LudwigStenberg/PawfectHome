
public interface IShelterService
{
    Task<ShelterResponse> RegisterShelterAsync(string userId, RegisterShelterRequest request);
    Task<ShelterResponse> GetShelterAsync(int id);
}