using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class SheltersController : ControllerBase
{

    private readonly IShelterService shelterService;
    public SheltersController(IShelterService shelterService)
    {
        this.shelterService = shelterService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateShelter(CreateShelterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await shelterService.RegisterShelterAsync(userId, request);
            // Make sure that service layer handles null

            // Reference GET method when it exists
            return CreatedAtAction(nameof(CreateShelter), new { Id = response.id }, response);

        }
        catch (Exception) // Waiting to implement until service method has been implemented.
        {

            throw;
        }
    }
}