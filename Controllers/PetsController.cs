using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
}
