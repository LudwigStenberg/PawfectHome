public class ShelterDetailResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Email { get; set; }
    public required string UserId { get; set; }
    public ICollection<PetSummaryResponse> Pets { get; set; } = new List<PetSummaryResponse>();
}
