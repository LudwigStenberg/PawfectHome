using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<AuthController> logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var response = await userService.RegisterUserAsync(request);
            return Ok(response);
        }
        catch (ValidationFailedException ex)
        {
            return BadRequest(new { Message = "Registration failed. Please check the errors.", Errors = ex.Errors });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during user registration");
            return StatusCode(500, "An unexpected error occurred during registration");
        }
    }
}
