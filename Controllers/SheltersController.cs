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
    public async Task<IActionResult> CreateShelter(RegisterShelterRequest request)
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

            return CreatedAtAction(nameof(GetShelter), new { id = response.Id }, response);

        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "An error occurred while saving to the database");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShelter(int id)
    {
        try
        {
            var response = await shelterService.GetShelterAsync(id);

            return Ok(response);

        } // TODO: Add specific exceptions
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
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
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}