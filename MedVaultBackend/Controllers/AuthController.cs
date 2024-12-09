using MedVaultBackend.Models;
using MedVaultBackend.Models.Requests;
using MedVaultBackend.Models.Response;
using MedVaultBackend.Script;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedVaultBackend.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(ApplicationContext context): ControllerBase
{
    public static readonly PasswordHasher<Users> passwordHasher = new();

    [HttpPost("registration/user")]
    public async Task<IActionResult> RegistrationNewUser(AddUser userData)
    {
        Users person = await context.Users.FirstOrDefaultAsync(u => u.Email == userData.Email);

        if (person is not null)
        {
            var errorResponce = new BaseResponce
            {
                Message = "Пользователь с данным email уже существует",
                Error = "Forbidden",
                ErrorCode = 403
            };
            return StatusCode(errorResponce.ErrorCode, errorResponce);
        }

        Users user = new Users
        {
            PersonId = Guid.NewGuid().ToString(),
            Name = userData.Name,
            Surname = userData.Surname,
            Patronymic = userData.Patronymic,
            Email = userData.Email,
            Birthday = userData.Birthday,
            CreateDate = DateTime.Now,
            PasswordVersion = 1
        };
        user.Password = passwordHasher.HashPassword(user, userData.Password);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        
        var tokens = TokenService.GenerateToken(user.PersonId, user.PasswordVersion);
        
        
        var responceResult = new RegistrationRequests
        {
            message = "Вы успешно создали аккаунт!",
            token_expires = 5,
            access_token = tokens.AccessToken,
            refresh_token = tokens.RefreshToken
        };
        
        return Ok(responceResult);
    }
    
    [HttpPost("authorization/user")]
    public async Task<IActionResult> AuthorizationUser(AuthUser userData)
    {
        Users? person = await context.Users.FirstOrDefaultAsync(u => u.Email == userData.Email);

        if (person is null || passwordHasher.VerifyHashedPassword(person, person.Password, userData.Password) != PasswordVerificationResult.Success)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Логин или пароль не верен",
                ErrorCode = 403,
                Error = "Forbidden",
                Success = false
            };
            return StatusCode(403, errorFailed);
        }
        
        var tokens = TokenService.GenerateToken(person.PersonId, person.PasswordVersion);
        
        var responceResult = new RegistrationRequests
        {
            message = "Вы успешно вошли в аккаунт",
            token_expires = 5,
            access_token = tokens.AccessToken,
            refresh_token = tokens.RefreshToken
        };
        
        return Ok(responceResult);
    }
    

    [Authorize]
    [HttpPut("refresh-token")]
    public IActionResult RefreshToken()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var person = TokenService.GetJwtTokenData(token);

        if (person.TokenType != TokenService.TokenType.refresh)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Отказано в доступе!",
                ErrorCode = 403,
                Error = "Forbidden",
                Success = false
            };
            return StatusCode(403, errorFailed);
        }
        
        var tokens = TokenService.GenerateToken(person.UserId, person.Version);
        
        var responceResult = new RegistrationRequests
        {
            message = "Вы успешно обновили токен",
            token_expires = 5,
            access_token = tokens.AccessToken,
            refresh_token = tokens.RefreshToken
        };

        return Ok(responceResult);
    }

    [Authorize]
    [HttpGet("check-auth")]
    public IActionResult CheckAuth()
    {
        return Ok();
    }
}