public class RegisterAdoptionResponse
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public AdoptionStatus AdoptionStatus { get; set; } = AdoptionStatus.Pending;
    public string UserId { get; internal set; }
    public int PetId { get; internal set; }
}