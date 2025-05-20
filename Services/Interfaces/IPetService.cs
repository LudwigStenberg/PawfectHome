public interface IPetService
{
    Task<GetPetResponse> GetPetAsync(int id);
    Task<RegisterPetResponse> RegisterPetAsync(RegisterPetRequest request);

    Task<IEnumerable<GetPetResponse>> GetAllPetsAsync();
    Task RemovePetAsync(int id);
}
