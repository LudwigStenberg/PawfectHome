using System.CodeDom.Compiler;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string id)
    {
        try
        {
            var userResponse = await userService.GetUserAsync(id, User);
            return Ok(userResponse);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while fetching user  ");
        }
    }
}
