using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            string? username = Request.Form["txtUserName"];
            string? password = Request.Form["txtPassword"];

            User? user = await _authenticationService.ValidateUserCredentialsAsync(username, password);
            if (user != null)
            {
                string? roleId = user.RoleId ?? string.Empty;
                string? userId = user.Id ?? string.Empty;
                string? roleName = user.Role!.RoleName ?? string.Empty;
                HttpContext.Session.SetString("user_id", userId);
                HttpContext.Session.SetString("role_name", roleName);
                HttpContext.Session.SetString("role_id", roleId);
                // Log session values to verify they're set
                Console.WriteLine($"User ID in session: {HttpContext.Session.GetString("user_id")}");
                Console.WriteLine($"Role ID in session: {HttpContext.Session.GetString("role_id")}");
                Response.Redirect("/");
            }
            else
            {
                Response.Redirect("/Error");
            }
        }

        public void onGetLogout()
        {
            HttpContext.Session.SetString("user_id", "");
            HttpContext.Session.SetString("role_id", "");
            Response.Redirect("/Login");
        }
    }
}
