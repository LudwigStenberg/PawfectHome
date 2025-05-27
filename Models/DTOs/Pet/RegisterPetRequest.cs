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
    public required string Birthdate { get; set; }

    [Range(0, 2)]
    public Gender Gender { get; set; } = Gender.Unknown;

    [Range(0, 2)]
    public Species Species { get; set; } = Species.Undefined;
    public string Breed { get; set; } = "Undefined";

    [StringLength(1000)]
    public string Description { get; set; } = "No description";
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; } = false;
    public bool HasPedigree { get; set; } = false;
    public int ShelterId { get; set; }
}
