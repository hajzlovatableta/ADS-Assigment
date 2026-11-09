using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebAPI.Mapping;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /auth/login
    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized("Username and password are required");
        }

        User? user = userRepo.GetMany().AsEnumerable()
            .SingleOrDefault(u => u.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            return Unauthorized("User not found");
        }

        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password");
        }

        // UserDto has no password, so nothing sensitive is sent back
        return Ok(DtoMapper.ToDto(user));
    }
}
