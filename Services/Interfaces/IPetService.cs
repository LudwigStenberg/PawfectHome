public interface IPetService
{
    Task<RegisterPetResponse> RegisterPetAsync(string userId, RegisterPetRequest request);
    Task<GetPetResponse> GetPetAsync(int id);
    Task<IEnumerable<GetPetResponse>> GetAllPetsAsync();
    Task<UpdatePetResponse> UpdatePetAsync(int petId, string userId, UpdatePetRequest request);
    Task RemovePetAsync(int id, string userId);
}
