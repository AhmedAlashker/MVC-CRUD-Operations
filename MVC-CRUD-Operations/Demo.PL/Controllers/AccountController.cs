using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailSettings _mailSettings;
        private readonly ISMSService _smsService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMailSettings mailSettings,
            ISMSService smsService)
        {
			_userManager = userManager;
            _signInManager = signInManager;
            _mailSettings = mailSettings;
            _smsService = smsService;
        }
		// Password => P@ssw0rd || Pa$$w0rd
		// BaseUrl/Account/Register
		#region Register
		public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) // Password => P@ssw0rd || Pa$$w0rd
        {
            if (ModelState.IsValid)
            {
                var User = new ApplicationUser()
                {
                    UserName = Regex.Replace(model.Email.Split('@')[0], @"\d", ""),
                    Email = model.Email,
                    IsAgree = model.IsAgree,
                    FName = model.FName,
                    LName = model.LName
                };

                var Result = await _userManager.CreateAsync(User, model.Password);

                if (Result.Succeeded)
                {
                    //return RedirectToAction("Login");
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in Result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        #endregion

        #region Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(model.Email);

                if (User is not null)
                {
                    var Result = await _userManager.CheckPasswordAsync(User, model.Password);
                    if (Result)
                    {
                        var LoginResult = await _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false);
                        if (LoginResult.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Password is Incorrect");
                }
                else
                    ModelState.AddModelError(string.Empty, "Email is Not Exist");
            }

            return View(model);
        }

        // Google Login
        public IActionResult GoogleLigin()
        {
            var prop = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        }

        // Google Response
        public async Task<IActionResult> GoogleResponse()
        {
            var Result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            var claims = Result.Principal.Identities.FirstOrDefault().Claims.Select(
                claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                }
                
                );

            return RedirectToAction("Index", "Home");
        }
		#endregion

		#region SignOut

		public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region ForgetPassword
        public IActionResult ForgetPassword()
        {
            return View();
        }

        #region Send Email Using (Mailkit, SMS[Twilio], Google, )
        
        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if (User is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(User);
        
                    /// https://localhost:5001/Account/ResetPassword?email=aalashker07@gmail.com
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = User.Email, Token = token }, Request.Scheme);
        
                    var email = new Email()
                    {
                        Subject = "Reset Password",
                        To = model.Email,
                        Body = ResetPasswordLink
                    };
                    //EmailSettings.SendEmail(email);
                    _mailSettings.SendMail(email);
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email is Not Exist");
                }
        	}
        	return View("ForgetPassword", model);
        }
        

        // Sneding Email Using SMS
        //[HttpPost]
        ///public async Task<IActionResult> SendSMS(ForgetPasswordViewModel model)
        ///{
        ///    if (ModelState.IsValid)
        ///    {
        ///        var User = await _userManager.FindByEmailAsync(model.Email);
        ///        if (User is not null)
        ///        {
        ///            var token = await _userManager.GeneratePasswordResetTokenAsync(User);
        ///
        ///
        ///            /// https://localhost:5001/Account/ResetPassword?email=aalashker07@gmail.com
        ///            var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = User.Email, Token = token }, Request.Scheme);
        ///
        ///            var SMS = new SMSMessage()
        ///            {
        ///                PhoneNumber = User.PhoneNumber,
        ///                Body = ResetPasswordLink
        ///            };
        ///            //EmailSettings.SendEmail(email);
        ///            //_mailSettings.SendMail(email);
        ///            _smsService.Send(SMS);
        ///
        ///            //return RedirectToAction(nameof(CheckYourInbox));
        ///            return Ok("Check Your Phone");
        ///        }
        ///        else
        ///        {
        ///            ModelState.AddModelError(string.Empty, "Email is Not Exist");
        ///        }
        ///    }
        ///    return View("ForgetPassword", model);
        ///} 
        

        #endregion


        public IActionResult CheckYourInbox()
        {
            return View();
        }

        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) 
        {
            if (ModelState.IsValid)
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string; 
                var User = await _userManager.FindByEmailAsync(email);
                var Result =  await _userManager.ResetPasswordAsync(User, token, model.NewPassword);
                if (Result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in Result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(model);
        }


		#endregion
	}
}
