public class GetAdoptionApplicationResponse
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public AdoptionStatus AdoptionStatus { get; set; }
    public string UserId { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; }
}