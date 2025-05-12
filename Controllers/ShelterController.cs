using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class ShelterController : ControllerBase
{

    private readonly IShelterService shelterService;
    public ShelterController(IShelterService shelterService)
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

            var response =

        }
        catch (Exception)
        {

            throw;
        }
    }


}