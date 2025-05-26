public interface IPetRepository
{
    public Task<PetEntity> CreatePetAsync(PetEntity petEntity);
    Task<PetEntity?> FetchPetAsync(int petId);
    Task<IEnumerable<PetEntity>> FetchAllPetsAsync();
    Task UpdatePetAsync(PetEntity existingPet);
    Task DeletePetAsync(PetEntity petEntity);
}
