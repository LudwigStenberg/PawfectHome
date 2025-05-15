public interface IPetService
{
    Task<GetPetResponse> GetPetAsync(int id);
    public Task<RegisterPetResponse> RegisterPetAsync(RegisterPetRequest request);
}
