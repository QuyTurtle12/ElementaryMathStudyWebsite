using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Services.Interface.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        // Constructor for dependency injection
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Generates a JWT token for a given user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>A JWT token as a string.</returns>
        public string GenerateJwtToken(User user)
        {
            // Retrieve the JWT secret from configuration
            var secret = _configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims based on user information
            var claims = new List<Claim>
            {
                new Claim("username", user.Username),
                new Claim("userId", user.Id.ToString())

            };

            // Retrieve the token expiry period from configuration
            var expiryInDays = int.Parse(_configuration["JwtSettings:ExpiryInDays"] ?? "1");

            // Create and return the JWT token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryInDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Decodes and validates a JWT token, returning the claims principal.
        /// </summary>
        /// <param name="token">The JWT token to decode.</param>
        /// <returns>The claims principal representing the token's claims.</returns>
        public ClaimsPrincipal DecodeJwtToken(string token)
        {
            // Retrieve the JWT secret from configuration
            var secret = _configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            // Set up token validation parameters
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            try
            {
                // Validate the token and return the claims principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new SecurityTokenException("Token has expired");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                throw new SecurityTokenException("Invalid token signature");
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid token");
            }
        }

        public Guid GetUserIdFromTokenHeader(string? token)
        {
            // Check if the token is null or empty
            if (string.IsNullOrEmpty(token))
            {
                return Guid.Empty; // Handle null or empty token gracefully
            }

            // Decode the JWT token and extract claims
            var principal = DecodeJwtToken(token);

            if (principal == null)
            {
                return Guid.Empty; // Handle null principal gracefully
            }

            // Extract claims from the principal
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedUserID))
            {
                return parsedUserID;
            }

            return Guid.Empty;
        }

    }
}
