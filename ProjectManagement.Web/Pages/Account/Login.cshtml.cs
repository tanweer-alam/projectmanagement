using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectManagement.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        public string? ReturnUrl { get; set; }
        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(GetSafeReturnUrl(returnUrl));
            }

            ReturnUrl = GetSafeReturnUrl(returnUrl);
            return Challenge(new AuthenticationProperties { RedirectUri = ReturnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult OnPost(string? returnUrl = null)
        {
            ReturnUrl = GetSafeReturnUrl(returnUrl);
            return Challenge(new AuthenticationProperties { RedirectUri = ReturnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        private string GetSafeReturnUrl(string? returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Content("~/");
        }
    }
}
