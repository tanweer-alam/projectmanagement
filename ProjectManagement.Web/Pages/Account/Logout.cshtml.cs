using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectManagement.Web.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "http://localhost:5066" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
                );
        }
    }}
