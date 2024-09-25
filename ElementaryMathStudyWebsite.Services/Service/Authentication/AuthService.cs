using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using System.Linq.Expressions;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class AuthService : IAppAuthService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IAuthenticationService authenticationService,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _unitOfWork = unitOfWork;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            User? user = await _authenticationService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!user.Status)
            {
                throw new UnauthorizedAccessException("User account is not active.");
            }

            return _authenticationService.GenerateJwtToken(user);
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            // Check if the user already exists
            var existingUser = await _unitOfWork.GetRepository<User>().FindByConditionAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            // Verify if the provided role name exists
            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(r => r.RoleName == "Parent");
            if (role == null)
            {
                throw new InvalidOperationException("Invalid role name.");
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create new user entity
            var newUser = new User
            {
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                Gender = registerDto.Gender,
                Email = registerDto.Email,
                Username = registerDto.Username,
                Password = hashedPassword,
                Status = false, // Set status to false until email is verified
                VerificationToken = Guid.NewGuid().ToString(), // Generate verification token
                Role = role // Set the role
            };
            newUser.CreatedBy = newUser.Id;
            

            // Save user to the database
            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            // Send verification email
            await _emailService.SendVerificationEmailAsync(registerDto.Email, newUser.VerificationToken);
        }

        public async Task StudentRegisterAsync(StudentRegisterDto registerDto, string email, string parentId)
        {
            // Check if the user already exists
            var existingUser = await _unitOfWork.GetRepository<User>().FindByConditionAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            // Verify if the provided role name exists
            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(r => r.RoleName == "Student");
            if (role == null)
            {
                throw new InvalidOperationException("Invalid role name.");
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create new user entity
            var newUser = new User
            {
                FullName = registerDto.FullName,
                Gender = registerDto.Gender,
                Username = registerDto.Username,
                Password = hashedPassword,
                Status = false, // Set status to false until email is verified
                VerificationToken = Guid.NewGuid().ToString(), // Generate verification token
                Role = role // Set the role
            };
            newUser.CreatedBy = parentId;
            // Save user to the database
            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            // Send verification email
            await _emailService.SendVerificationEmailAsync(email, newUser.VerificationToken);
        }

        public async Task VerifyEmailAsync(string token)
        {
            Expression<Func<User, bool>> condition = u => u.VerificationToken == token;

            var user = await _unitOfWork.GetRepository<User>().FindByConditionWithIncludesAsync(
                condition,
                u => u.Role // Include the Role if needed
                // Add other includes here if needed
            );

            if (user == null)
            {
                throw new InvalidOperationException("Invalid verification token.");
            }

            // Activate the user account
            user.Status = true;
            if (!user.Role.RoleName.Equals("Student"))
            {
                user.CreatedBy = user.Id;
            }

            user.VerificationToken = null; // Clear the verification token
            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }
    }
}
