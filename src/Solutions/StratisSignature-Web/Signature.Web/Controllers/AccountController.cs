using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Signature.BAL.Interface;
using Signature.Shared.Models;
using Signature.Utility;
using Signature.Web.Models;

namespace Signature.Web.Controllers
{
    public class AccountController : Controller
    {
        #region Properties

        private readonly IUserService _userService;
        private readonly IToastNotification _toastNotification;

        #endregion

        #region Constructor

        public AccountController(IUserService userService, IToastNotification toastNotification)
        {
            _userService = userService;
            _toastNotification = toastNotification;
        }

        #endregion

        #region NonAction

        [NonAction]
        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(Constants.UserIdType, Convert.ToString(user.UserId)),
                new Claim(Constants.FirstNameType, user.FirstName),
                new Claim(Constants.LastNameType,user.LastName),
                new Claim(Constants.WalletAddressType,user.WalletAddress)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                IssuedUtc = DateTime.UtcNow,
                IsPersistent = false,
                AllowRefresh = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        #endregion

        #region Methods

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {            
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmail(model.Email);
                if (!string.IsNullOrEmpty(user.Password))
                {
                    var passwordHashValid = PasswordHasher.VerifyIdentityV3Hash(model.Password, user.Password);
                    if (passwordHashValid)
                    {
                        await SignInUserAsync(user);
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                _toastNotification.AddErrorToastMessage(Constants.AccountLoginInvalidCredential);
                return View();
            }
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = PasswordHasher.GenerateIdentityV3Hash(model.Password),
                    WalletAddress = model.WalletAddress
                };

                var result = await _userService.RegisterUser(user);
                if (result > 0)
                {
                    _toastNotification.AddSuccessToastMessage(Constants.AccountRegistrationSuccess);
                    return RedirectToAction("login");
                }
            }
            return View(model);
        }

        #endregion
    }
}