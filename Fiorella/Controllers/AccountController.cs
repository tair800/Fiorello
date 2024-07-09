using Fiorella.Helpers;
using Fiorella.Models;
using Fiorella.Services.Interfaces;
using Fiorella.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Fiorella.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);
            AppUser user = new()
            {
                UserName = registerVM.UserName,
                FullName = registerVM.FullName,
                Email = registerVM.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);//instead of "Identity" we can use "var"
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }
            //send verify email
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(VerifyEmail), "Account", new { email = user.Email, token = token },
                Request.Scheme, Request.Host.ToString());

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/templates/emailTemplate/emailConfirm.html"))
            {
                body = reader.ReadToEnd();
            };
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{username}}", user.FullName);
            _emailService.SendEmail(new() { user.Email }, body, "Email verification", "Verify email");


            await _userManager.AddToRoleAsync(user, nameof(RolesEnum.Member));
            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return NotFound();
            await _userManager.ConfirmEmailAsync(appUser, token);
            await _signInManager.SignInAsync(appUser, true);
            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
                if (user == null)
                {

                    ModelState.AddModelError("", "Username or Email is wrong...");
                    return View(loginVM);

                }
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (result.IsLockedOut)
            {
                //todo:add lockout minutes
                ModelState.AddModelError("", "Account is Locked for a while ");
                return View(loginVM);
            }
            if (user.IsBlocked)
            {
                ModelState.AddModelError("", "Account is Blocked for a while ");
                return View(loginVM);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Verification needed");
                return View(loginVM);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is wrong");
                return View(loginVM);
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("admin")) return RedirectToAction("index", "dashboard", new { area = "adminarea" });


            return RedirectToAction("index", "home");
        }


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> AddRole()
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
                await _roleManager.CreateAsync(new IdentityRole { Name = "admin" });
            if (!await _roleManager.RoleExistsAsync("member"))
                await _roleManager.CreateAsync(new IdentityRole { Name = "member" });
            if (!await _roleManager.RoleExistsAsync("superadmin"))
                await _roleManager.CreateAsync(new IdentityRole { Name = "superadmin" });
            return Content("Roles added");

        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser is null)
            {
                ModelState.AddModelError("", "Given email does not exist");
                return View();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            string url = Url.Action(nameof(ResetPassword), "Account"
                , new { email = appUser.Email, token = token }
                , Request.Scheme
                , Request.Host.ToString());


            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/templates/passwordTemplate/forgetPassword.html"))
            {
                body = reader.ReadToEnd();
            };
            body = body.Replace("{{link}}", url);
            body = body.Replace("{{username}}", appUser.FullName);

            _emailService.SendEmail(new() { appUser.Email }, body, "Forget password", "Reset password");

            return RedirectToAction("index", "home");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token, ResetPasswordVM resetPasswordVM)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);

            if (!ModelState.IsValid) return View();

            await _userManager.ResetPasswordAsync(appUser, token, resetPasswordVM.Password);

            return RedirectToAction("login", "account");
        }

    }
}
