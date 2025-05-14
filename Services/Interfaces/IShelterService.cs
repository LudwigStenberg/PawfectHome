
public interface IShelterService
{
    Task<RegisterShelterResponse> RegisterShelterAsync(string userId, RegisterShelterRequest request);
}