using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ToDoApp.Models;
using ToDoApp.Services;
using ToDoApp.ViewModel;

namespace ToDoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly EmailService emailService;
        private readonly ProfileServices profileServices;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailService emailService,
            ProfileServices profileServices)
        {
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.profileServices = profileServices;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel { Step = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Email
            // Password 
            // ConfirmPassword
            // Name
            // VerificationCode
            switch (model.Step)
            {
                case 1:
                    // first step Remove from Model State the Password and ConfirmPassword and VerificationCode
                    ModelState.Remove("VerificationCode");
                    break;
                case 2:
                    // second step Remove from Model State the Password and ConfirmPassword and Name and Email
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPassword");
                    ModelState.Remove("Name");
                    ModelState.Remove("Email");
                    break;
                default:
                    throw new InvalidOperationException("There is only Three Steps");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            switch (model.Step)
            {
                case 1:
                    // first step take the Name / Email
                    var verificationCode = GenerateVerificationCode();
                    await emailService.SendEmail(model.Email, verificationCode, model.Name);

                    TempData["RegisterData"] = JsonSerializer.Serialize(model);
                    TempData["VerificationCode"] = verificationCode;

                    TempData.Keep("RegisterData");
                    TempData.Keep("VerificationCode");

                    return View(new RegisterViewModel
                    {
                        Step = 2,
                        Email = model.Email
                    });
                case 2:
                    // second step take verefication code
                    TempData.Keep("RegisterData");
                    TempData.Keep("VerificationCode");

                    string x = TempData["RegisterData"]?.ToString() ?? "Hamdy";
                    var storedData = JsonSerializer.Deserialize<RegisterViewModel>(x);
                    var storedCode = TempData["VerificationCode"]?.ToString() ?? "Hamdy";

                    if (model.VerificationCode != storedCode)
                    {
                        ModelState.AddModelError("VerificationCode", "Invalid verification code");
                        return View(new RegisterViewModel
                        {
                            Step = 2,
                            Email = storedData?.Email ?? "Invlid",
                            VerificationCode = model.VerificationCode
                        });
                    }

                    ApplicationUser user = new ApplicationUser();
                    // Email
                    // Password 
                    // ConfirmPassword
                    // Name
                    // VerificationCode
                    user.Email = storedData?.Email;
                    user.UserName = storedData?.Name ?? "No Name";

                    var result = await userManager.CreateAsync(user, storedData?.Password ?? "12345678");

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return NotFound();
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                default:
                    throw new InvalidOperationException("There is only Two Steps");
            }
        }
        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
        [HttpGet]
        public IActionResult Forgot()
        {
            return View(new ForgotVM { Step = 1 });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(ForgotVM forgotVM)
        {
            switch (forgotVM.Step)
            {
                case 1:
                    ModelState.Remove("VerificationCode");
                    if (!ModelState.IsValid)
                    {
                        return View(forgotVM);
                    }

                    var verificationCode = GenerateVerificationCode();
                    await emailService.SendEmail(forgotVM.Email, verificationCode, forgotVM.Email);

                    TempData["Email"] = forgotVM.Email;
                    TempData["VerificationCode"] = verificationCode;

                    return View(new ForgotVM
                    {
                        Step = 2,
                        Email = forgotVM.Email
                    });
                case 2:
                    ModelState.Remove("Email");
                    if (!ModelState.IsValid)
                    {
                        return View(forgotVM);
                    }
                    var storedData = TempData["Email"]?.ToString();
                    var storedCode = TempData["VerificationCode"]?.ToString();

                    if (forgotVM.VerificationCode != storedCode)
                    {
                        ModelState.AddModelError("VerificationCode", "Invalid verification code");
                        return View(new ForgotVM
                        {
                            Step = 2,
                            Email = storedData ?? "Hamdy"
                        });
                    }
                    ApplicationUser user = await userManager.FindByEmailAsync(storedData ?? "") ?? new ApplicationUser();
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    // Code verified - redirect to password reset
                    return RedirectToAction("ResetPassword", new { Email = storedData, Code = code });

                default:
                    return View(forgotVM);
            }
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string code)
        {
            return View(new ResetPasswordVM
            {
                Email = email,
                Code = code
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return RedirectToAction("Login");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", new { message = "Password reset successfully" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM userLogin)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? userDb = await userManager.FindByEmailAsync(userLogin.Email);
                if (userDb != null)
                {
                    if (userDb.IsBlocked)
                    {
                        ModelState.AddModelError("", "You are Blocked");
                        return View(userLogin);
                    }
                    bool passwordIsCorrect = await userManager.CheckPasswordAsync(userDb, userLogin.Password);
                    if (passwordIsCorrect)
                    {
                        await signInManager.SignInAsync(userDb, isPersistent: userLogin.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Username or Password is Invalid");
            }
            return View(userLogin);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Profile()
        {
            Claim? id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return RedirectToAction("Login", "Acount");
            }
            ProfileVM profileVM = profileServices.GetProfileInfo(id.Value);
            return View(profileVM);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePassword { Step = 1 });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword model, bool resendCode = false)
        {
            switch (model.Step)
            {
                case 1:
                    ModelState.Remove("VerificationCode");
                    if (!ModelState.IsValid)
                        return View(model);
                    break;
                case 2:
                    if (model.VerificationCode == null)
                        return View(model);
                    break;
            }
            switch (model.Step)
            {
                case 1:
                    TempData["Email"] = model.Email;
                    TempData["CurrentPassword"] = model.CurrentPassword;
                    TempData["NewPassword"] = model.NewPassword;

                    string code = GenerateVerificationCode();
                    TempData["VerificationCode"] = code;
                    await emailService.SendEmail(model.Email, code, model.Email);

                    return View(
                        new ChangePassword()
                        {
                            Step = 2,
                            Email = model.Email
                        }
                    );
                case 2:
                    string storedEmail = TempData["Email"]?.ToString() ?? "";
                    string storedCurrentPassword = TempData["CurrentPassword"]?.ToString() ?? "";
                    string storedNewPassword = TempData["NewPassword"]?.ToString() ?? "";
                    string storedCode = TempData["VerificationCode"]?.ToString() ?? "";
                    if (model.VerificationCode != storedCode)
                    {
                        ModelState.AddModelError("VerificationCode", "Invalid verification code");
                        return View(new RegisterViewModel
                        {
                            Step = 2,
                            Email = storedEmail,
                            VerificationCode = model.VerificationCode
                        });
                    }


                    var user = await userManager.FindByEmailAsync(storedEmail);
                    if (user == null)
                    {
                        return RedirectToAction("Login");
                    }

                    var result = await userManager.ChangePasswordAsync(user, storedCurrentPassword, storedNewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", new { message = "Password Changed successfully" });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                default:
                    return NotFound();
            }
       }
    }
}