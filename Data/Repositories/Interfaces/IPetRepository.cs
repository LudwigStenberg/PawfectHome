public interface IPetRepository
{
    Task<PetEntity> FetchPetAsync(int petId);

    public Task<PetEntity> CreatePetAsync(PetEntity petEntity);

    Task<IEnumerable<PetEntity>> FetchAllPetsAsync();
}
