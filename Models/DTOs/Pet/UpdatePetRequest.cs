public class UpdatePetRequest
{
    public string? Name { get; set; }
    public DateTime? Birthdate { get; set; }
    public Gender? Gender { get; set; }
    public Species? Species { get; set; }
    public string? Breed { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool? IsNeutered { get; set; }
    public bool? HasPedigree { get; set; }
}