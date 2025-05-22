using System.ComponentModel.DataAnnotations;

public class UpdatePetRequest
{
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "The pet name must be between 3 and 50 characters."
    )]
    public string? Name { get; set; }
    public DateTime? Birthdate { get; set; }
    public Gender? Gender { get; set; }
    public Species? Species { get; set; }
    public string? Breed { get; set; }

    [StringLength(maximumLength: 1000)]
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool? IsNeutered { get; set; }
    public bool? HasPedigree { get; set; }
}
