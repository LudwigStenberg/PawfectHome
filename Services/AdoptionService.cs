public class AdoptionService : IAdoptionService
{

    private readonly AppDbContext dbContext;
    private readonly IAdoptionRepository adoptionRepository;

    public AdoptionService(AppDbContext appdbContext, IAdoptionRepository adoptionRepository)
    {
        dbContext = appdbContext;
        this.adoptionRepository = adoptionRepository;
    }

    public async Task<RegisterAdoptionResponse> RegisterAdoptionApplicationAsync(RegisterAdoptionRequest request)
    {
        var AdoptionApplicationEntity = new AdoptionApplicationEntity
        {
            UserId = request.UserId,
            PetId = request.PetId
        };

        var createdAdoptionApplication = await adoptionRepository.RegisterAdoptionApplicationAsync(AdoptionApplicationEntity);

        var response = new RegisterAdoptionResponse
        {

            Id = createdAdoptionApplication.Id,
            CreatedDate = createdAdoptionApplication.DateTime.UtcNow,
            AdoptionStatus = createdAdoptionApplication.AdoptionStatus.Pending,
        };

        return response;
    }

}