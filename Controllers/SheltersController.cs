using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class SheltersController : ControllerBase
{
    private readonly IShelterService shelterService;
    private readonly ILogger<SheltersController> logger;

    public SheltersController(IShelterService shelterService, ILogger<SheltersController> logger)
    {
        this.shelterService = shelterService;
        this.logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateShelter(RegisterShelterRequest request)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var (shelter, authChanged) = await shelterService.RegisterShelterAsync(userId, request);

            var response = new ApiResponse<RegisterShelterResponse>(shelter);
            response.Meta.AuthenticationChanged = authChanged;

            return CreatedAtAction(nameof(GetShelter), new { id = shelter.Id }, response);
        }
        catch (UserIdRequiredException)
        {
            return BadRequest();
        }
        catch (ValidationFailedException ex)
        {
            logger.LogDebug("Validation failed for shelter creation: {@Errors}", ex.Errors);
            return BadRequest(new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors });
        }
        catch (MultipleSheltersNotAllowedException)
        {
            return Conflict();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while creating shelter for user {UserId}", userId);
            return StatusCode(500, "An unexpected error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShelter(int id)
    {
        try
        {
            var response = await shelterService.GetShelterAsync(id);
            return Ok(response);
        }
        catch (ShelterNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while retrieving shelter with ID: {ShelterId}",
                id
            );
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShelters()
    {
        try
        {
            var response = await shelterService.GetAllSheltersAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex, "An unexpected error occurred while attempting to retrieve all shelters");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> UpdateShelter(int id, [FromBody] ShelterUpdateRequest request)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var response = await shelterService.UpdateShelterAsync(id, userId, request);
            return Ok(response);
        }
        catch (UserIdRequiredException)
        {
            return BadRequest();
        }
        catch (ValidationFailedException ex)
        {
            logger.LogDebug("Validation failed for shelter update: {@Errors}", ex.Errors);
            return BadRequest(new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors });
        }
        catch (ShelterNotFoundException)
        {
            return NotFound();
        }
        catch (ShelterOwnershipException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while updating shelter with ID: {ShelterId} by user {UserId}",
                id,
                userId
            );
            return StatusCode(500, "An unexpected error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> DeleteShelter(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await shelterService.RemoveShelterAsync(id, userId);
            return NoContent();
        }
        catch (ShelterNotFoundException)
        {
            return NotFound();
        }
        catch (ShelterOwnershipException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while deleting shelter with ID {ShelterId} by user {UserId}",
                id,
                userId
            );
            return StatusCode(500, "An unexpected error occurred while processing your request.");
        }
    }
}
