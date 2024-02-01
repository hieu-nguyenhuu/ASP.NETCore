using ASPNetCoreJwtAuth.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace ASPNetCoreJwtAuth.Services
{
    public class SignInManager
    {
        private readonly ILogger<SignInManager> _logger;
        private readonly ApplicationDbContext _ctx;
        private readonly JWTAuthService _JwtAuthService;
        private readonly JwtTokenConfig _jwtTokenConfig;

        public SignInManager(ILogger<SignInManager> logger,
                             JWTAuthService JWTAuthService,
                             JwtTokenConfig jwtTokenConfig,
                             ApplicationDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
            _JwtAuthService = JWTAuthService;
            _jwtTokenConfig = jwtTokenConfig;
        }

        public async System.Threading.Tasks.Task<SignInResult> SignIn(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]", userName);

            SignInResult result = new SignInResult();

            if (string.IsNullOrWhiteSpace(userName)) return result;
            if (string.IsNullOrWhiteSpace(password)) return result;

            var user = await _ctx.Users.Where(f => f.Email == userName && f.Password == password).FirstOrDefaultAsync();
            if (user != null)
            {

                var claims = BuildClaims(user);
                result.User = user;
                result.AccessToken = _JwtAuthService.BuildToken(claims);
                result.RefreshToken = _JwtAuthService.BuildRefreshToken();

                _ctx.RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = result.RefreshToken, IssuedAt = DateTime.Now, ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration) });
                _ctx.SaveChanges();

                result.Success = true;
            };

            return result;
        }

        public async System.Threading.Tasks.Task<SignInResult> RefreshToken(string AccessToken, string RefreshToken)
        {

            ClaimsPrincipal claimsPrincipal = _JwtAuthService.GetPrincipalFromToken(AccessToken);
            SignInResult result = new SignInResult();

            if (claimsPrincipal == null) return result;

            string id = claimsPrincipal.Claims.First(c => c.Type == "id").Value;
            var user = await _ctx.Users.FindAsync(Convert.ToInt32(id));

            if (user == null) return result;

            var token = await _ctx.RefreshTokens
                    .Where(f => f.UserId == user.Id
                            && f.Token == RefreshToken
                            && f.ExpiresAt >= DateTime.Now)
                    .FirstOrDefaultAsync();

            if (token == null) return result;

            var claims = BuildClaims(user);

            result.User = user;
            result.AccessToken = _JwtAuthService.BuildToken(claims);
            result.RefreshToken = _JwtAuthService.BuildRefreshToken();

            _ctx.RefreshTokens.Remove(token);
            _ctx.RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = result.RefreshToken, IssuedAt = DateTime.Now, ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration) });
            _ctx.SaveChanges();

            result.Success = true;

            return result;
        }

        private Claim[] BuildClaims(User user)
        {
            //User is Valid
            var claims = new[]
            {
                new Claim("id",user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Email)
 
                //Add Custom Claims here
            };

            return claims;
        }

    }

    public class SignInResult
    {
        public bool Success { get; set; }
        public User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public SignInResult()
        {
            Success = false;
        }
    }
}