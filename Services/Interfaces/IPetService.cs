public interface IPetService
{
    Task<RegisterPetResponse> RegisterPetAsync(RegisterPetRequest request, string userId);
    Task<GetPetResponse> GetPetAsync(int id);
    Task<IEnumerable<GetPetResponse>> GetAllPetsAsync();
    Task<UpdatePetResponse> UpdatePetAsync(int petId, string userId, UpdatePetRequest request);
    Task RemovePetAsync(int id, string userId);
}
