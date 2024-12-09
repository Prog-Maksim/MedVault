using MedVaultBackend.Models.Response;
using MedVaultBackend.Script;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedVaultBackend.Controllers;


[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public class CategoriesController(ApplicationContext context): ControllerBase
{
    [Authorize]
    [HttpGet("get-all-doctorName")]
    public async Task<IActionResult> GetAllCategoriesNameAsync()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        TokenService.JwtTokenData person;
        
        try
        {
            person = TokenService.GetJwtTokenData(token);
        }
        catch (ArgumentException)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Отказано в доступе!",
                ErrorCode = 401,
                Error = "Unauthorized",
                Success = false
            };
            return StatusCode(401, errorFailed);
        }
        
        if (person.TokenType != TokenService.TokenType.access)
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

        var results = await context.Documents.Where(d => d.DoctorName != null).Select(d => d.DoctorName).Distinct().ToListAsync();
        
        return Ok(results);
    }
    
    
    [Authorize]
    [HttpGet("get-all-doctorSpeciality")]
    public async Task<IActionResult> GetAllCategoriesSpecialityAsync()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        TokenService.JwtTokenData person;
        
        try
        {
            person = TokenService.GetJwtTokenData(token);
        }
        catch (ArgumentException)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Отказано в доступе!",
                ErrorCode = 401,
                Error = "Unauthorized",
                Success = false
            };
            return StatusCode(401, errorFailed);
        }
        
        if (person.TokenType != TokenService.TokenType.access)
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
        
        var results = await context.Documents.Where(d => d.DoctorSpecialty != null).Select(d => d.DoctorSpecialty).Distinct().ToListAsync();
        
        return Ok(results);
    }
    
    
    [Authorize]
    [HttpGet("get-doctorNameOrSpeciality")]
    public async Task<IActionResult> GetCategoriesNameOrSpecialityAsync()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        TokenService.JwtTokenData person;
        
        try
        {
            person = TokenService.GetJwtTokenData(token);
        }
        catch (ArgumentException)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Отказано в доступе!",
                ErrorCode = 401,
                Error = "Unauthorized",
                Success = false
            };
            return StatusCode(401, errorFailed);
        }
        
        if (person.TokenType != TokenService.TokenType.access)
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

        var result = await context.Documents.Where(d => d.DoctorName != null)
            .Select(d => new { d.DoctorName, d.DoctorSpecialty }).ToListAsync();
        
        
        return Ok(result);
    }
    
    
    [Authorize]
    [HttpGet("get-all-documentType")]
    public async Task<IActionResult> GetAllCategoriesDocumentTypeAsync()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        TokenService.JwtTokenData person;
        
        try
        {
            person = TokenService.GetJwtTokenData(token);
        }
        catch (ArgumentException)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Отказано в доступе!",
                ErrorCode = 401,
                Error = "Unauthorized",
                Success = false
            };
            return StatusCode(401, errorFailed);
        }
        
        if (person.TokenType != TokenService.TokenType.access)
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
        
        var results = await context.Documents.Select(d => d.DocumentType).Distinct().ToListAsync();
        
        return Ok(results);
    }
    
    
    [Authorize]
    [HttpGet("get-all-analyzes")]
    public async Task<IActionResult> GetAllCategoriesAnalyzesAsync()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        TokenService.JwtTokenData person;
        
        try
        {
            person = TokenService.GetJwtTokenData(token);
        }
        catch (ArgumentException)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Отказано в доступе!",
                ErrorCode = 401,
                Error = "Unauthorized",
                Success = false
            };
            return StatusCode(401, errorFailed);
        }
        
        if (person.TokenType != TokenService.TokenType.access)
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
        
        var results = await context.Documents.Where(d => d.Surveys != null).Select(d => d.Surveys).Distinct().ToListAsync();
        
        return Ok(results);
    }
}