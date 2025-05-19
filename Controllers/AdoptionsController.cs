using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AdoptionsController : ControllerBase
{
    private readonly ILogger<AdoptionsController> logger;
    private readonly IAdoptionService adoptionService;

    public AdoptionsController(IAdoptionService adoptionService, ILogger<AdoptionsController> logger)
    {
        this.adoptionService = adoptionService;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new adoption application.
    /// </summary>
    /// <param name="request">The adoption application details.</param>
    /// <returns>The created adoption application.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAdoptionApplication([FromBody] RegisterAdoptionRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var result = await adoptionService.RegisterAdoptionApplicationAsync(request);
            logger.LogInformation(
                "Adoption application with ID {Id} successfully created for UserId: {UserId}",
                result.Id,
                userId
            );
            return CreatedAtAction(nameof(CreateAdoptionApplication), new { id = result.Id }, result);
        }
        catch (ValidationFailedException ex)
        {
            logger.LogWarning(
                ex,
                "Validation failed while creating an adoption application for UserId: {UserId}. Errors: {@Errors}",
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
                "Entity not found while creating an adoption application for UserId: {UserId}. Message: {Message}",
                userId,
                ex.Message
            );
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while creating an adoption application for UserId: {UserId}. Message: {Message}",
                userId,
                ex.Message
            );
            return StatusCode(500, "An unexpected error occurred while registering the adoption application.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAdoptionApplication(int id)
    {
        try
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await RemoveAdoptionApplicationAsync(id, userId);
            return NoContent();
        }
        catch (Exception)
        {

            throw;
        }

    }
}