using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PetsController : ControllerBase
{
    private readonly ILogger<PetsController> logger;
    private readonly IPetService petService;

    public PetsController(IPetService petService, ILogger<PetsController> logger)
    {
        this.petService = petService;
        this.logger = logger;
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
            return StatusCode(500, "Unexpected error occured while fetching pet");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetPetResponse>>> GetAllPets()
    {
        var pets = await petService.GetAllPetsAsync();

        return Ok(pets);
    }

    [HttpPost]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> CreatePet([FromBody] RegisterPetRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await petService.RegisterPetAsync(request, userId);
            return CreatedAtAction(nameof(GetPet), new { id = result.Id }, result);
        }
        catch (ValidationFailedException ex)
        {
            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occured while registering the pet.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> DeletePet(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            await petService.RemovePetAsync(id, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occured while deleting the pet.");
        }
    }
}
