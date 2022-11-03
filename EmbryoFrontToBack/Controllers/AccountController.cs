using EmbryoFrontToBack.Models;
using EmbryoFrontToBack.Services.Interfaces;
using EmbryoFrontToBack.ViewModels.AccountViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmbryoFrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _singInManager;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> singInManager,
            IEmailService emailService,
            IFileService fileService)
            
        {
            _userManager = userManager;
            _singInManager = singInManager;
            _emailService = emailService;
            _fileService = fileService;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid)
            {
                View(register);
            }

            AppUser user = new AppUser
            {
                Fullname = register.Fullname,
                UserName = register.Username,
                Email = register.Email,


            };

            IdentityResult result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View(register);
                }
            }

            //await _singInManager.SignInAsync(user, false);
            //return RedirectToAction("Index", "Home");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token },
                Request.Scheme, Request.Host.ToString());

            string subject = "Verify Email";
            string path = "wwwroot/assets/Templates/Verify.html";
            string body = string.Empty;

            body = _fileService.ReadFile(path, body);

            body = body.Replace("{{Link}}", link);
            body = body.Replace("{{fullname}}", user.Fullname);

            _emailService.Send(user.Email, subject, body);

            return RedirectToAction(nameof(VerifyEmail));
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId is null || token == null) return BadRequest();

            AppUser appUser = await _userManager.FindByIdAsync(userId);

            if (appUser is null) return NotFound();

            await _userManager.ConfirmEmailAsync(appUser, token);

            await _singInManager.SignInAsync(appUser, false);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _singInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                View(login);
            }

            AppUser user = await _userManager.FindByEmailAsync(login.UsernameOrEmail);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(login.UsernameOrEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Email Or Passwor Wrong");
                return View(login);
            }

            var result = await _singInManager.PasswordSignInAsync(user, login.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email Or Passwor Wrong");
                return View(login);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
