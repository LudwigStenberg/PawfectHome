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

    [Authorize]
    [HttpPost]
    public IActionResult CreatePet(RegisterPetRequest request)
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
