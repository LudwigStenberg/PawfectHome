
public interface IShelterService
{
    Task<ShelterResponse> RegisterShelterAsync(string userId, RegisterShelterRequest request);
}