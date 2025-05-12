
public interface IShelterRepository
{
    Task<ShelterEntity> CreateShelterAsync(ShelterEntity newShelter);
}