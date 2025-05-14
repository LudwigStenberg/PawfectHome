
public interface IShelterService
{
    Task<RegisterShelterResponse> RegisterShelterAsync(string userId, RegisterShelterRequest request);
    Task<ShelterResponse> GetShelterAsync(int id);
    Task<ICollection<ShelterResponse>> GetAllShelters();
}