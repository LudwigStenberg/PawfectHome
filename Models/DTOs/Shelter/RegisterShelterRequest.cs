
using System.ComponentModel.DataAnnotations;

public class RegisterShelterRequest
{
    [Required(ErrorMessage = "Shelter name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Shelter name must be between 3 and 50 characters.")]
    public required string Name { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address")]
    public required string Email { get; set; }
}