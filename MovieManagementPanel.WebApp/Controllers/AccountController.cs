using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.WebApp.Models;
using System.Security.Claims;

namespace MovieManagementPanel.WebApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        #region Ctor&Fields

        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(ILogger<AccountController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserLoginModel model, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users
                .Find(i => i.IsActive && i.Email == model.Email && i.Password == model.Password)
                .AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View(model);
            }
            List<Claim> userClaims = new();
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            userClaims.Add(new Claim(ClaimTypes.Name, user.FullName));
            userClaims.Add(new Claim(ClaimTypes.Email, user.Email));

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.IsRememberMe
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(TempData["ReturnUrl"]?.ToString()))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
                return Redirect(TempData["ReturnUrl"].ToString());
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

    }
}
