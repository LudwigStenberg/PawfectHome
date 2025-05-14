using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PetsController : ControllerBase
{
    private readonly IPetService petService;

    public PetsController(IPetService petService)
    {
        this.petService = petService;
    }

    /// <summary>
    /// Get pet by id.
    /// </summary>
    /// <param name="id"> Unique identifier of the pet id.</param>
    /// <returns>The pet if found otherwise </returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPet(int id)
    {
        try
        {
            var pet = await petService.GetPetAsync(id);

            return Ok(pet);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Your pet is dead...bitch");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePet(RegisterPetRequest request)
    {
        try
        {
            var result = petService.RegisterPetAsync(request);
            return CreatedAtAction(nameof(GetPet), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            // TODO: Implement logging.
            return StatusCode(500, "An error occurred while registering the pet.");
        }
    }
}
