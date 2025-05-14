public interface IPetService
{
    Task<GetPetResponse> GetPetAsync(int id);
}
