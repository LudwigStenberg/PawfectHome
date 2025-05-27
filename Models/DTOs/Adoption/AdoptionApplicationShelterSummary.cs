public class AdoptionApplicationShelterSummary
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public AdoptionStatus AdoptionStatus { get; set; }
    public string ApplicantName { get; set; }
    public string PetName { get; set; }
    public int PetId { get; set; }
}
