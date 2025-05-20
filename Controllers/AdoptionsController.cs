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

    [HttpGet("getById/{id}")]
    [Authorize]
    public async Task<IActionResult> GetAdoptionApplication(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var request = new GetAdoptionApplicationRequest
            {
                Id = id,
                UserId = userId
            };
            var response = await adoptionService.GetAdoptionApplicationAsync(request);


            if (userId != response.UserId && !User.IsInRole("Admin") && !User.IsInRole("Shelter"))
            {
                logger.LogWarning(
                    "User {UserId} attempted to access adoption application {Id} belonging to another user",
                    userId,
                    id
                );
                return StatusCode(403, "You have no access to this application.");
            }

            logger.LogInformation(
                "Adoption application with ID {Id} successfully retrieved",
                id
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Adoption application with ID {Id} was not found. Requested by user: {UserId}. Message: {Message}",
                id,
                userId,
                ex.Message
            );

            return NotFound(new { Message = ex.Message });
        }
        catch (ValidationFailedException ex)
        {
            logger.LogWarning(
                ex,
                "Validation failed while retrieving adoption application with ID {Id} for user: {UserId}. Errors: {@Errors}",
                id,
                userId,
                ex.Errors
            );

            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while retrieving adoption application with ID {Id} for user: {UserId}. Message: {Message}",
                id,
                userId,
                ex.Message
            );

            return StatusCode(500, "An unexpected error occurred while retrieving the adoption application.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAdoptionApplication(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await adoptionService.RemoveAdoptionApplicationAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex, "An unexpected error occurred while deleting adoption application with ID {AdoptionApplicationId} by user {UserId}", id, userId);

            return StatusCode(500, "An unexpected error occurred while deleting the adoption application.");
        }
    }
}