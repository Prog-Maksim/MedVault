using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace MedVaultBackend.Script;

public class TokenService
{
    private const int AccessTokenLifetimeMinute = 3000;
    private const int RefreshTokenLifetimeDay = 30;

    private static List<JwtRefreshToken> RefreshTokens = new();
    
    private static string GenerateJwtAccessToken(string userId)
    {
        string jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim("token_type", TokenType.access.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenLifetimeMinute),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    private static string GenerateJwtRefreshToken(string userId, string deviceId, int passwordVersion, out string jti)
    {
        jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim("device", deviceId),
            new Claim("token_type", TokenType.refresh.ToString()),
            new Claim("version", passwordVersion.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };
        
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(RefreshTokenLifetimeDay),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static JwtTokenData GetJwtTokenData(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        
        if (!handler.CanReadToken(token))
            throw new ArgumentException("Неверный jwt токен");
        
        var jwtToken = handler.ReadJwtToken(token);
        
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var deviceId = jwtToken.Claims.FirstOrDefault(c => c.Type == "device")?.Value;
        var token_type = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
        var tokenType = Enum.Parse<TokenType>(token_type);
        var versionClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "version")?.Value;
        var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        
        int version = 0;
        if (int.TryParse(versionClaim, out var parsedVersion))
            version = parsedVersion;
        
        return new JwtTokenData
        {
            UserId = userId,
            DeviceId = deviceId,
            TokenType = tokenType,
            Version = version,
            Jti = jti,
        };
    }

    public static JwtToken GenerateToken(string userId, int passwordVersion)
    {
        string jti;
        string deviceId = Guid.NewGuid().ToString();
        
        string refreshToken = GenerateJwtRefreshToken(userId, deviceId, passwordVersion, out jti);
        string accessToken = GenerateJwtAccessToken(userId);

        

        JwtRefreshToken token = new JwtRefreshToken
        {
            Jti = jti,
            DeviceId = deviceId,
            Revoke = false
        };
        RefreshTokens.Add(token);
        

        JwtToken jwtToken = new JwtToken
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return jwtToken;
    }
    
    
    public class JwtRefreshToken
    {
        public string Jti { get; set; }
        public string DeviceId { get; set; }
        public bool Revoke { get; set; }
    }

    public static JwtToken RefreshJwtToken(string accessToken, string refreshToken)
    {
        if (!CheckAccessToken(accessToken))
            throw new SecurityTokenException("Invalid token");
        
        if (!CheckAccessToken(refreshToken))
            throw new SecurityTokenException("Invalid token");

        var jwtData = GetJwtTokenData(refreshToken);
        
        var token = GenerateToken(jwtData.UserId, jwtData.Version);

        RevokeRefreshToken(refreshToken);
        AddBlackListAccessToken(accessToken);
        
        return token;
    }
    
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    
    public static void RevokeRefreshToken(string refreshToken)
    {
        // TODO: Добавить аннулирование токена
    }
    
    public static void AddBlackListAccessToken(string accessToken)
    {
        // TODO: Добавить, добавление токена в черный список
    }
    
    public static bool CheckAccessToken(string accessToken)
    {
        // TODO: Добавить проверку валидности токена
        
        return true;
    }
    
    
    public enum TokenType
    {
        access,
        refresh
    }
    
    public class JwtTokenData
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public TokenType TokenType { get; set; }
        public int Version { get; set; }
        public string Jti { get; set; }
    }
}
