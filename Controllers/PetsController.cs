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

    [HttpGet("get-pet/{id}")]
    public async Task<IActionResult> GetPet(int id)
    {
        try
        {
            var pet = await petService.GetPetAsync(id);

            if (pet == null)
            {
                return NotFound($"Pet not found");
            }

            return Ok(pet);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}
