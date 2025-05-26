using System.ComponentModel.DataAnnotations;

public class UpdateAdoptionStatusRequest
{
    public int Id { get; set; }

    [Required]
    [Range(0, 2)]
    public AdoptionStatus AdoptionStatus { get; set; }
}
