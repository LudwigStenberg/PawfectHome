public interface IPetService
{
    Task<PetEntity> GetPetAsync(int id);
    public Task<RegisterPetResponse> RegisterPetAsync(RegisterPetRequest request);
}
