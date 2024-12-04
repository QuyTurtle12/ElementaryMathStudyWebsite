namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    using Google.Apis.Auth;
    using ElementaryMathStudyWebsite.Contract.Core.IUOW;
    using ElementaryMathStudyWebsite.Core.Repositories.Entity;
    using System;
    using System.Threading.Tasks;
    using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
    using ElementaryMathStudyWebsite.Core.Base;
    using Microsoft.Extensions.Configuration; // Add this import for IConfiguration
    using System.Text.Json;

    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailService _emailService;
        private readonly string _googleClientId;
        private readonly IConfiguration _configuration;

        // Modify constructor to accept IConfiguration
        public GoogleAuthService(IUnitOfWork unitOfWork,
                                 IAuthenticationService authenticationService,
                                 IEmailService emailService,
                                 IConfiguration configuration) // Inject IConfiguration
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _emailService = emailService;
            _googleClientId = configuration["GoogleAuth:ClientId"] ?? ""; // Get client ID from appsettings
            _configuration = configuration;
        }

        public async Task<string> LoginWithGoogleAsync(string idToken)
        {
            // Validate the token using the client ID from appsettings
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleClientId } // Use the injected client ID
            });

            if (payload == null)
            {
                throw new Exception("Invalid Google ID Token.");
            }

            // Verify if the provided role name exists
            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(r => r.RoleName == "Parent");
            if (role == null)
            {
                throw new BaseException.CoreException("invalid_argument", "Invalid role name.");
            }

            // Check if the user exists in the database
            var user = await _unitOfWork.GetRepository<User>().FindByConditionAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                // Register the user if not exists
                var newUser = new User
                {
                    FullName = payload.Name,
                    Email = payload.Email,
                    Username = payload.Email,
                    Password = null!, // Password not required for Google login
                    Status = true,   // Google accounts are considered verified
                    CreatedBy = "GoogleLogin",
                    Role = role
                };

                await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
                await _unitOfWork.SaveAsync();

                user = newUser;
            }

            // Generate JWT for the user
            var jwtToken = _authenticationService.GenerateJwtToken(user);
            return jwtToken;
        }

        public async Task<string> ExchangeCodeForIdToken(string code)
        {
            var client = new HttpClient();
            var requestData = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _configuration["GoogleAuth:ClientId"] },
                { "client_secret", _configuration["GoogleAuth:ClientSecret"] },
                { "redirect_uri", "https://localhost:7137/api/auth/google-login" },  // The same URI you used in the Google OAuth setup
                { "grant_type", "authorization_code" }
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON response to extract the ID token
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                return jsonResponse.GetProperty(" ").GetString();
            }
            else
            {
                throw new Exception("Failed to exchange code for ID token.");
            }
        }
    }
}
