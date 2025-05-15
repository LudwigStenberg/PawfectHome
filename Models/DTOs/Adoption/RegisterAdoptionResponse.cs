public class RegisterAdoptionResponse
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public AdoptionStatus AdoptionStatus { get; set; } = AdoptionStatus.Pending;
}