using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

public class RegisterPetRequest
{
    [Required(ErrorMessage = "Name must be provided")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "The pet name must be between 3 and 50 characters."
    )]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Birthdate must be provided")]
    public string Birthdate { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public Species Species { get; set; } = Species.Undefined;
    public string Breed { get; set; } = "Undefined";

    [StringLength(maximumLength: 1000)]
    public string Description { get; set; } = "No description";
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; } = false;
    public bool HasPedigree { get; set; } = false;
    public int ShelterId { get; set; }
}
