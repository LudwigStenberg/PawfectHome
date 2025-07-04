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
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (PetNotFoundException)
        {
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
        catch (AdoptionApplicationOwnershipException)
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

    [HttpGet("shelter")]
    [Authorize(Roles = "ShelterOwner")]
    public async Task<IActionResult> GetAllShelterAdoptionApplication()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        try
        {
            var applications = await adoptionService.GetAllShelterAdoptionApplicationsAsync(userId);
            return Ok(applications);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        catch
        {
            return StatusCode(500);
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
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
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
