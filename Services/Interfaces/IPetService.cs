public interface IPetService
{
    Task<PetEntity> GetPetAsync(int id);
}
