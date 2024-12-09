using System.Security.Cryptography;
using MedVaultBackend.Models.DB;
using MedVaultBackend.Models.Requests;
using MedVaultBackend.Models.Response;
using MedVaultBackend.Script;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedVaultBackend.Controllers;

[ApiController]
[Route("api/document")]
[Produces("application/json")]
public class DocumentController(ApplicationContext context): ControllerBase
{
    [Authorize]
    [HttpPost("add-document")]
    public async Task<IActionResult> AddDocument(AddDocument document)
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

        try
        {
            Documents doc = new Documents
            {
                DocumentId = await GenerateUniqueDocumentId(person.UserId),
                PersonId = person.UserId,
                DocumentAdded = DateTime.Now,
                DateAdmission = document.DateAdmission,
                DoctorName = document.DoctorName,
                DoctorSpecialty = document.DoctorSpecialty,
                DocumentType = document.DocumentType,
                Surveys = document.Analyzes,
                Price = document.Price
            };

            await context.Documents.AddAsync(doc);
            await context.SaveChangesAsync();

            return Ok(new { documentId = doc.DocumentId });
        }
        catch (InvalidOperationException)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Не удается создать документ. Достигнут лимит на количество документов.",
                ErrorCode = 409,
                Error = "Conflict",
                Success = false
            };
            return StatusCode(errorFailed.ErrorCode, errorFailed);
        }
    }
    
    private static Dictionary<string, HashSet<int>> _userDocumentCache = new ();

    private async Task<int> GenerateUniqueDocumentId(string personId)
    {
        int documentId = 0;
        bool isUnique = false;
        int maxAttempts = 10000;
        int attemptCount = 0;

        if (!_userDocumentCache.ContainsKey(personId))
        {
            _userDocumentCache[personId] = new HashSet<int>(await context.Documents
                .Where(d => d.PersonId == personId)
                .Select(d => d.DocumentId)
                .ToListAsync());
        }

        Console.WriteLine(_userDocumentCache[personId].Count);
        if (_userDocumentCache[personId].Count >= 9999)
            throw new InvalidOperationException();
        
        while (!isUnique)
        {
            documentId = GenerateRandomNumber(1, 9999);

            if (!_userDocumentCache[personId].Contains(documentId))
                isUnique = true;
            else
            {
                attemptCount++;
                if (attemptCount >= maxAttempts)
                    throw new InvalidOperationException();
            }
        }

        _userDocumentCache[personId].Add(documentId);
        return documentId;
    }
    
    private static int GenerateRandomNumber(int min, int max)
    {
        byte[] randomNumber = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        
        int result = Math.Abs(BitConverter.ToInt32(randomNumber, 0));
        
        return result % (max - min) + min;
    }
    
    [Authorize]
    [HttpGet("get-document")]
    public async Task<IActionResult> GetDocument([FromQuery]int documentId)
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
        
        var res = await context.Documents
            .Include(d => d.MedicalInstitution)
            .FirstOrDefaultAsync(d => d.PersonId == person.UserId && d.DocumentId == documentId);
        
        if (res is null)
        {
            BaseResponce errorFailed = new BaseResponce
            {
                Message = "Данный документ не найден!",
                ErrorCode = 404,
                Error = "Not Fount",
                Success = false
            };
            return StatusCode(errorFailed.ErrorCode, errorFailed);
        }

        DocumentResponse doc = new DocumentResponse
        {
            DocumentId = res.DocumentId,
            DateAdmission = res.DateAdmission,
            DoctorName = res.DoctorName,
            DoctorSpecialty = res.DoctorSpecialty,
            DocumentType = res.DocumentType,
            Price = res.Price,
            Analyzes = res.Surveys,
            Adress = res.Address,
            Name = res.MedicalInstitution.Name,
            City = res.MedicalInstitution.City,
            Street = res.MedicalInstitution.Street
        };
        
        return Ok(doc);
    }

    [Authorize]
    [HttpGet("search-document")]
    public async Task<IActionResult> GetDocuments([FromQuery] DateTime? dateStart, [FromQuery] DateTime? dateEnd,
        [FromQuery] string? doctorName, [FromQuery] string? doctorSpecialty, [FromQuery] string? documentType, [FromQuery] string? analyzes,
        [FromQuery] double? priceStart, [FromQuery] double? priceEnd)
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
        
        var query = context.Documents.AsQueryable();
        query = query.Where(d => d.PersonId == person.UserId);
        query = query.Include(d => d.MedicalInstitution);
 
        if (dateStart.HasValue)
            query = query.Where(d => d.DateAdmission >= dateStart.Value);
        if (dateEnd.HasValue)
            query = query.Where(d => d.DateAdmission <= dateEnd.Value);
        if (!string.IsNullOrEmpty(doctorName))
            query = query.Where(d => d.DoctorName == doctorName);
        if (!string.IsNullOrEmpty(doctorSpecialty))
            query = query.Where(d => d.DoctorSpecialty == doctorSpecialty);
        if (!string.IsNullOrEmpty(documentType))
            query = query.Where(d => d.DocumentType == documentType);
        if (!string.IsNullOrEmpty(analyzes))
                query = query.Where(d => d.Surveys == analyzes);
        if (priceStart.HasValue)
            query = query.Where(d => d.Price >= priceStart);
        if (priceEnd.HasValue)
            query = query.Where(d => d.Price <= priceEnd);
        
        var documents = await query.Select(d => new DocumentResponse
            {
                DocumentId = d.DocumentId,
                DateAdmission = d.DateAdmission,
                DoctorName = d.DoctorName,
                DoctorSpecialty = d.DoctorSpecialty,
                DocumentType = d.DocumentType,
                Price = d.Price,
                Analyzes = d.Surveys,
                Name = d.MedicalInstitution.Name,
                City = d.MedicalInstitution.City,
                Street = d.MedicalInstitution.Street
            })
            .OrderBy(d => d.DocumentId)
            .ToListAsync();

        return Ok(documents);
    }

    [Authorize]
    [HttpGet("get-all-documents")]
    public async Task<IActionResult> GetAllDocuments()
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

        var documents = await context.Documents
            .Include(d => d.MedicalInstitution)
            .Where(d => d.PersonId == person.UserId)
            .Select(d => new DocumentResponse
            {
                DocumentId = d.DocumentId,
                DateAdmission = d.DateAdmission,
                DoctorName = d.DoctorName,
                DoctorSpecialty = d.DoctorSpecialty,
                DocumentType = d.DocumentType,
                Price = d.Price,
                Analyzes = d.Surveys,
                Name = d.MedicalInstitution.Name,
                City = d.MedicalInstitution.City,
                Street = d.MedicalInstitution.Street
            }).ToListAsync();

        return Ok(documents);
    }
    
    
    [Authorize]
    [HttpGet("get-num-documents")]
    public async Task<IActionResult> GetNumDocuments()
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

        var documentNums = await context.Documents.Where(d => d.PersonId == person.UserId).CountAsync();

        return Ok(documentNums);
    }
}