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

            // Reference GET method when it exists
            return CreatedAtAction(nameof(CreateShelter), new { id = response.Id }, response);

        } // TODO: Make sure that the catches matches the thrown exceptions and that the correct status codes are returned
        catch (DbUpdateException)
        {
            // 500 + failure message
        }
        catch (ValidationException)
        {
            // BadRequest
        }
        catch (Exception)
        {

            // 500 + unexpected error
        }
    }
}