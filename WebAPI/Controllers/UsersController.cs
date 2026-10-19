using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebAPI.Exceptions;
using WebAPI.Mapping;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        ValidateUser(request.UserName, request.Password);
        VerifyUserNameIsAvailable(request.UserName);

        User user = new() { UserName = request.UserName, Password = request.Password };
        User created = await userRepo.AddAsync(user);

        UserDto dto = DtoMapper.ToDto(created);
        return Created($"/Users/{dto.Id}", dto);
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        ValidateUser(request.UserName, request.Password);

        User existing = await userRepo.GetSingleAsync(id);

        if (!existing.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase))
        {
            VerifyUserNameIsAvailable(request.UserName);
        }

        existing.UserName = request.UserName;
        existing.Password = request.Password;
        await userRepo.UpdateAsync(existing);

        return NoContent();
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingleUser([FromRoute] int id)
    {
        User user = await userRepo.GetSingleAsync(id);
        return Ok(DtoMapper.ToDto(user));
    }

    // GET /users?userName=an
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetManyUsers([FromQuery] string? userName)
    {
        IEnumerable<User> users = userRepo.GetMany().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(userName))
        {
            users = users.Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));
        }

        List<UserDto> result = users.Select(DtoMapper.ToDto).ToList();
        return Ok(result);
    }

    // DELETE /users/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser([FromRoute] int id)
    {
        await userRepo.DeleteAsync(id);
        return NoContent();
    }

    private void VerifyUserNameIsAvailable(string userName)
    {
        bool taken = userRepo.GetMany().AsEnumerable()
            .Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

        if (taken)
        {
            throw new BadRequestException($"Username '{userName}' is already taken");
        }
    }

    private static void ValidateUser(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("Username cannot be empty");
        if (string.IsNullOrWhiteSpace(password))
            throw new BadRequestException("Password cannot be empty");
    }
}