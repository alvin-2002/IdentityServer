using System.Security.Claims;
using IdentityServerWeb.DTO;
using IdentityServerWeb.Entities;
using IdentityServerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerWeb.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<Account> _userManager;
        private readonly TokenService _tokenService;
        private readonly SendGridEmail _sendGridEmail;

        public AccountController(
            UserManager<Account> userManager, 
            TokenService tokenService, 
            SendGridEmail sendGridEmail) 
        {
            _sendGridEmail = sendGridEmail;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AccountDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized();
            }

            return new AccountDto
            {
                Email = user.Email ?? "",
                Token = _tokenService.GenerateToken(user)
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new Account {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            return StatusCode(201);
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<AccountDto>> GetCurrentUser()
        {

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

            var account = await _userManager.FindByEmailAsync(userEmail);

            if (account == null)
            {
                return NotFound("Current User not found");
            }

            return new AccountDto
            {
                Email = account.Email ?? "",
                Token = _tokenService.GenerateToken(account)
            };
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (forgotPasswordDto == null)
            {
                throw new ArgumentNullException("ForgotPasswordDto Object cannot be null");
            }

            if (string.IsNullOrEmpty(forgotPasswordDto.EmailAddress))
            {
                throw new ArgumentNullException(nameof(forgotPasswordDto.EmailAddress));
            }

            Account? account = await _userManager.FindByEmailAsync(forgotPasswordDto.EmailAddress);
         
            if (account == null)
            {
                return NotFound("Account not found");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);

            // callbackurl for rerouting
            var callbackurl = Url.Action("ResetPassword", "Account", new { userId = account.Id, code = code }, protocol: HttpContext.Request.Scheme);

            await _sendGridEmail.SendEmailAsync(forgotPasswordDto.EmailAddress, "Reset Email Confirmation", "Please reset email by going to this " +
                    callbackurl);

            return Ok();
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {

            var account = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (account == null)
            {
                return NotFound("Account not found");
            }

            // Reset password
            var result = await _userManager.ResetPasswordAsync(account, resetPasswordDto.Code, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            return Ok();
        }
    }
}