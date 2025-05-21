public interface IPetService
{
    Task<GetPetResponse> GetPetAsync(int id);
    Task<RegisterPetResponse> RegisterPetAsync(RegisterPetRequest request, string userId);
    Task<IEnumerable<GetPetResponse>> GetAllPetsAsync();
    Task RemovePetAsync(int id, string userId);
}
