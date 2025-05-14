
public interface IShelterService
{
    Task<CreateShelterResponse> RegisterShelterAsync(string userId, CreateShelterRequest request);
}