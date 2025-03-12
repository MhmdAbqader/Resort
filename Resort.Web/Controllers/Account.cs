using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Resort.Application.Repository;
using Resort.Application.Services;
using Resort.Application.Utility;
using Resort.Domain.Models;
using Resort.Web.ViewModels;

namespace Resort.Web.Controllers
{
    public class Account : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public Account(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
   

        public IActionResult Register(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            RegisterVM registerModel = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Select(a => new SelectListItem { Value = a.Name, Text = a.Name }),
                RedirectUrl = returnUrl
            };
            return View(registerModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    Name = register.Name,
                    Email = register.Email,
                    UserName = register.Email,
                    PhoneNumber = register.PhoneNumber,
                    NormalizedEmail = register.Email.ToUpper(),
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user,register.Password);
                if (result.Succeeded)
                {
                    if (register.Role is not null)
                        await _userManager.AddToRoleAsync(user, register.Role);
                    else
                        await _userManager.AddToRoleAsync(user, SD.CustomerRole);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (register.RedirectUrl is null)
                        return RedirectToAction("Index", "Home");
                    else
                        return LocalRedirect(register.RedirectUrl);                        
                }

                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

            }
 
            RegisterVM registerModel = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Select(a => new SelectListItem { Value = a.Name, Text = a.Name })
            };
            return View(registerModel);
        }


        public IActionResult Login(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            //if(returnUrl == null) 
            //    returnUrl = Url.Content("~/");

            var loginModel = new LoginVM()
            {
                RedirectUrl = returnUrl
            };

            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginModel)
        {
            if (ModelState.IsValid)
            {
               var result = await _signInManager
                    .PasswordSignInAsync(loginModel.Email,loginModel.Password,loginModel.RememberMe,false);
                if (result.Succeeded)
                {
                    //OK working
                    //if (User.IsInRole(SD.AdminRole))
                    //    return RedirectToAction("AdminDashborad", "Dashborad");

                    // also, this is working well but this is the best practice
                    //var user = _unitOfWork.ApplicationUserRepository.GetById("give userId from principleClaims to get current user");  
                    ApplicationUser? user = await _userManager.FindByEmailAsync(loginModel.Email);
                    if (await _userManager.IsInRoleAsync(user,SD.AdminRole)) 
                    {
                        return RedirectToAction("AdminDashboradIndex", "Dashboard");
                    }
                    else
                    {
                        if (loginModel.RedirectUrl is null)
                            return RedirectToAction("Index", "Home");
                        else
                            return LocalRedirect(loginModel.RedirectUrl);
                    }
                }
                else
                    ModelState.AddModelError("", "Attempt to Login is Invalid !");
            }

          

            return View(loginModel);
             
        }
        public async Task<IActionResult> LogOut() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {           
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVeiwModel changePasswordVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser? user = await _userManager.FindByIdAsync(currentUserId);
            if (user is not null && changePasswordVM.CurrentPassword is not null) 
            {
                var result = await _userManager.CheckPasswordAsync(user, changePasswordVM.CurrentPassword);
                if (result)
                {
                    var newUserChangedPassword = await _userManager.ChangePasswordAsync(user, changePasswordVM.CurrentPassword, changePasswordVM.NewPassword); ;
                    if (newUserChangedPassword.Succeeded)
                    {
                        TempData["success"] = "Password has been changed Successfully!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError("", "Attempt to change password is failed!");
                }
                else
                    ModelState.AddModelError("", "Current Password Incorrect !");

            }
            TempData["error"] = "Error occued, try again after a few minutes!";
            return View(changePasswordVM);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email , string phone)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            //ApplicationUser? user = await _userManager.FindByIdAsync(currentUserId);
            // if user send only his email =>>>>> it can be send email for another Customer so that i enforce him to also send phone
            // to ensure that is the real customer 
              ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user is not null && user.PhoneNumber == phone)
            {
                user.PasswordHash = null;
                var removeUserPassword =await _userManager.RemovePasswordAsync(user);

                // generate a random password made of 1CapitalLetter 1No only 1of(!@#_) 6LowercaseLetter
                string randomPassword = GenerateRandomPassword();

                var addUserPassword = await _userManager.AddPasswordAsync(user, randomPassword);
                if (addUserPassword.Succeeded)
                {
                    await _userManager.UpdateAsync(user);
                    // send test password for Accessing Resort Website in email 
                    string body = "your Password has been reseted successfuly <b style = 'color:blue;text-decoration:underline'> "
                        + randomPassword + "</b> you can change it for more security and avoiding security issues ";
                    string subject = "Forgot Password";
                    bool isSent = await _emailService.SendOneEmailAsync("Mhmd.Abqader@outlook.com", subject, body);
                    if (isSent)
                    {
                        TempData["success"] = "Check Mail.. Password reset Successfully!";
                        return RedirectToAction("Index", "Home");
                    }
                }
           
            }
            TempData["error"] = "Error occued, try again after a few minutes!";
            return View();

        }
       private static string GenerateRandomPassword()
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            // Define character sets
            string capitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowerLetters = "abcdefghijklmnopqrstuvwxyz";
            string specialChars = "!@#_";
            string numbers = "0123456789";

            // Add one random capital letter
            sb.Append(capitalLetters[random.Next(capitalLetters.Length)]);


            for (int i = 0; i < 6; i++)
            {
                sb.Append(lowerLetters[random.Next(lowerLetters.Length)]);
            }

            sb.Append(numbers[random.Next(numbers.Length)]);

            sb.Append(specialChars[random.Next(specialChars.Length)]);

            // Shuffle the string to ensure randomness in the order
            string result = ShuffleString(sb.ToString(), random);

            return result;
        }

       private static string ShuffleString(string str, Random random)
        {
            // Convert the string to a character array
            char[] array = str.ToCharArray();

            // Shuffle the array using Fisher-Yates algorithm
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                // Swap characters
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            // Convert the shuffled array back to a string
            return new string(array);
        }
    }
}
