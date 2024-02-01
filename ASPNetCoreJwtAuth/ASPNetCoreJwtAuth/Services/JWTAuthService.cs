using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ASPNetCoreJwtAuth.Services
{

    public class JWTAuthService
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly ILogger<JWTAuthService> _logger;

        public JWTAuthService(JwtTokenConfig jwtTokenConfig,
                              ILogger<JWTAuthService> logger)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _logger = logger;
        }

        public string BuildToken(Claim[] claims) // create access token
        {
            // Tạo khóa bảo mật đối xứng từ chuỗi bí mật
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));

            // Tạo thông tin xác thực để ký token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token JWT
            var token = new JwtSecurityToken(
                    issuer: _jwtTokenConfig.Issuer,
                    audience: _jwtTokenConfig.Audience,
                    notBefore: DateTime.Now,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                    signingCredentials: creds);

            // Chuyển đổi token thành chuỗi JWT
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string BuildRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //Phương thức GetPrincipalFromToken() được sử dụng để xác thực và trích xuất thông tin xác thực (claims) từ một token JWT.
        //Nếu xác thực thành công, nó sẽ trả về một đối tượng ClaimsPrincipal chứa thông tin về người dùng hoặc đối tượng được biểu diễn bởi token.
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            JwtSecurityTokenHandler tokenValidator = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Thiết lập các tham số xác thực token
            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false, //không kiểm tra audience
                ValidateIssuer = false, //không kiểm tra Issuer-người phát hành
                ValidateIssuerSigningKey = true, //kiểm tra signingkey
                IssuerSigningKey = key, //khoá bí mật để xác thực chữ ký token
                ValidateLifetime = false //không kiểm tra thời gian token
            };

            try
            {
                var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogError($"Token validation failed");
                    return null;
                }

                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError($"Token validation failed: {e.Message}");
                return null;
            }
        }
    }




    public class JwtTokenConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }

    }

}