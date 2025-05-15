using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class AdoptionsController : ControllerBase
{

    private readonly IAdoptionService adoptionService;

    public AdoptionsController(IAdoptionService adoptionsService)
    {
        this.adoptionService = adoptionService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAdoptionApplication([FromBody] RegisterAdoptionRequest request)
    {
        try
        {
            var result = adoptionService.RegisterAdoptionApplicationAsync(request);
            // return CreatedAtAction(nameof(GetAdoptionApplication), new { id = result.Id }, result);
            return (IActionResult)result;
        }

        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while registering the adoptionapplication.");
        }
    }
}