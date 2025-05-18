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
            return StatusCode(500, "Your pet is dead...bitch");
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
        try
        {
            var result = await petService.RegisterPetAsync(request);
            logger.LogInformation(
                "Pet with ID {Id} successfully created for UserId: {UserId}",
                result.Id,
                userId
            );
            return CreatedAtAction(nameof(GetPet), new { id = result.Id }, result);
        }
        catch (ValidationFailedException ex)
        {
            logger.LogWarning(
                ex,
                "Validation failed while creating a pet for UserId: {UserId}. Errors: {@Errors}",
                userId,
                ex.Errors
            );
            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Shelter not found while creating a pet for UserId: {UserId}. Message: {Message}",
                userId,
                ex.Message
            );

            return NotFound(new { Message = "The specified shelter could not be found." });
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occure while creating a pet for UserId: {UserId}. Message: {Message}",
                userId,
                ex.Message
            );
            return StatusCode(500, "An unexpected error occured while registering the pet.");
        }
    }
}
