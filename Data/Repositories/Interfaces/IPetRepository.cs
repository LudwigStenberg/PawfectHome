public interface IPetRepository
{
    Task<PetEntity> FetchPetAsync(int petId);

    Task<PetEntity> CreatePetAsync();
}
