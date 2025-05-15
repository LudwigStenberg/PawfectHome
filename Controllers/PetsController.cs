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

            // 404 Not Found with a clear message
            return NotFound(new { Message = "The specified shelter could not be found."});
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
