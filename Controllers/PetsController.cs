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

    [HttpPost]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> CreatePet([FromBody] RegisterPetRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await petService.RegisterPetAsync(userId, request);
            return CreatedAtAction(nameof(GetPet), new { id = result.Id }, result);
        }
        catch (ValidationFailedException ex)
        {
            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (ShelterOwnershipException)
        {
            return Forbid();
        }
        catch (ShelterNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while registering pet. RequestedBy: {UserId}",
                userId
            );
            return StatusCode(500, "An unexpected error occured while registering the pet.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPet(int id)
    {
        try
        {
            var pet = await petService.GetPetAsync(id);

            return Ok(pet);
        }
        catch (PetNotFoundException)
        {
            return NotFound();
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

    [HttpPut("{id}")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> UpdatePet(int id, [FromBody] UpdatePetRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var response = await petService.UpdatePetAsync(id, userId, request);
            return Ok(response);
        }
        catch (ValidationFailedException ex)
        {
            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (ShelterOwnershipException)
        {
            return Forbid();
        }
        catch (Exception ex) when (ex is PetNotFoundException || ex is ShelterNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while updating pet. PetId: {PetId}, RequestedBy: {UserId}",
                id,
                userId
            );
            return StatusCode(500, "An unexpected error occured while updating the pet.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> DeletePet(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await petService.RemovePetAsync(id, userId);
            return NoContent();
        }
        catch (ShelterOwnershipException)
        {
            return Forbid();
        }
        catch (Exception ex) when (ex is PetNotFoundException || ex is ShelterNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while deleting pet. PetId: {PetId}. RequestedBy: {UserId}",
                id,
                userId
            );
            return StatusCode(500, "An unexpected error occured while deleting the pet.");
        }
    }
}
