using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
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

            User? user = await _authenticationService.ValidateUserCredentialsAsync(username ?? "", password ?? "");
            if (user != null)
            {
                string? roleId = user.RoleId ?? string.Empty;
                string? userId = user.Id ?? string.Empty;
                string? userFullname = user.FullName ?? string.Empty;
                string? roleName = user.Role!.RoleName ?? string.Empty;
                HttpContext.Session.SetString("user_id", userId);
                HttpContext.Session.SetString("user_fullname", userFullname);
                HttpContext.Session.SetString("role_name", roleName);
                HttpContext.Session.SetString("role_id", roleId);
                HttpContext.Session.SetString("user_status", user.Status.ToString() ?? "false");
                Response.Redirect("/");
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid username or password."; // Set error message
            }
        }

        public void onGetLogout()
        {
            HttpContext.Session.Clear();
            Response.Redirect("/Login");
        }
    }
}
