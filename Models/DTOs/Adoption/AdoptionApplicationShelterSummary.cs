public class AdoptionApplicationShelterSummary
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public AdoptionStatus AdoptionStatus { get; set; }
    public required string ApplicantName { get; set; }
    public required string PetName { get; set; }
    public int PetId { get; set; }
}
