using System.ComponentModel.DataAnnotations;

public class RegisterPetRequest
{
    [Required(ErrorMessage = "Name cannot be null")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "The pet name must be between 3 and 50 characters."
    )]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Age cannot be null")]

    public int Age { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public Species Species { get; set; } = Species.Undefined;
    public string Breed { get; set; } = "Undefined";
    public string Description { get; set; } = "No description";
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; } = false;
    public bool HasPedigree { get; set; } = false;
    public int ShelterId { get; set; }
}
