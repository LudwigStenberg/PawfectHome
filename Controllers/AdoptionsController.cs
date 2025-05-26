using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AdoptionsController : ControllerBase
{
    private readonly ILogger<AdoptionsController> logger;
    private readonly IAdoptionService adoptionService;

    public AdoptionsController(
        IAdoptionService adoptionService,
        ILogger<AdoptionsController> logger
    )
    {
        this.adoptionService = adoptionService;
        this.logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAdoptionApplication(
        [FromBody] RegisterAdoptionRequest request
    )
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await adoptionService.RegisterAdoptionApplicationAsync(request, userId);
            logger.LogInformation(
                "Adoption application with ID {Id} successfully created for UserId: {UserId}",
                result.Id,
                userId
            );
            return CreatedAtAction(nameof(GetAdoptionApplication), new { id = result.Id }, result);
        }
        catch (ValidationFailedException ex)
        {
            logger.LogWarning(
                ex,
                "Validation failed while creating adoption application for UserId: {UserId}. Errors: {@Errors}",
                userId,
                ex.Errors
            );
            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (UserNotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "User not found while creating an adoption application for UserId: {UserId}. Message: {Message}",
                userId,
                ex.Message
            );
            return NotFound();
        }
        catch (PetNotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Pet not found while creating an adoption application for UserId: {UserId}. Message: {Message}",
                userId,
                ex.Message
            );

            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while creating adoption application for UserId: {UserId}",
                userId
            );
            return StatusCode(
                500,
                "An unexpected error occurred while creating the adoption application."
            );
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetAdoptionApplication(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var response = await adoptionService.GetAdoptionApplicationAsync(id, userId);
            return Ok(response);
        }
        catch (AdoptionApplicationNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ValidationFailedException ex)
        {
            return BadRequest(
                new { Message = "Validation failed. Please check the errors.", Errors = ex.Errors }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while retrieving adoption application with ID {Id} for user {UserId}",
                id,
                userId
            );
            return StatusCode(
                500,
                "An unexpected error occurred while retrieving the adoption application."
            );
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<
        ActionResult<IEnumerable<GetAdoptionApplicationResponse>>
    > GetAdoptionApplications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        try
        {
            var applications = await adoptionService.GetAllAdoptionApplicationsAsync(userId);
            logger.LogInformation(
                "Successfully retrieved {Count} adoption applications for user {UserId}",
                applications.Count(),
                userId
            );
            return Ok(applications);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while retrieving adoption applications for user {UserId}",
                userId
            );
            return StatusCode(
                500,
                "An unexpected error occurred while retrieving adoption applications."
            );
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> UpdateAdoptionStatus(
        int id,
        [FromBody] UpdateAdoptionStatusRequest request
    )
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await adoptionService.UpdateAdoptionStatusAsync(id, request, userId);
            return Ok(result);
        }
        catch
        {
            return StatusCode(500);
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
        catch (AdoptionApplicationNotFoundException)
        {
            return NotFound();
        }
        catch (AdoptionApplicationOwnershipException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred while deleting adoption application with ID {AdoptionApplicationId} by user {UserId}",
                id,
                userId
            );

            return StatusCode(
                500,
                "An unexpected error occurred while deleting the adoption application."
            );
        }
    }
}
