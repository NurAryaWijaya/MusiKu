using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MusiKu_API.Models;
using MusiKu_API.OTP;
using MusiKu_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MusiKu_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _memoryCache;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration config, IEmailService emailService, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _memoryCache = memoryCache;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Validasi email harus diakhiri dengan @gmail.com
            if (!registerDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Email harus menggunakan domain @gmail.com");
            }

            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                return BadRequest(new { errors = errorMessages });
            }


            return Ok(new { message = "User created successfully" });
        }


        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Identifier) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest("Identifier dan password harus diisi.");

            IdentityUser user;

            // Deteksi apakah identifier berupa email
            if (loginDto.Identifier.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(loginDto.Identifier);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginDto.Identifier);
            }

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized("Invalid username/email or password");

            // Buat token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var token = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.UtcNow.AddYears(10),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                username = user.UserName,
                email = user.Email,
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost("send-reset-code")]
        public async Task<IActionResult> SendResetCode([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found");

            var otp = new Random().Next(100000, 999999).ToString();

            // Simpan kode dan waktu expired (misalnya 10 menit)
            OtpMemoryStore.OtpCodes[model.Email] = (otp, DateTime.UtcNow.AddMinutes(10));

            var subject = "Your OTP Reset Code";
            var body = $"Your password reset code is: <b>{otp}</b>";

            await _emailService.SendEmailAsync(model.Email, subject, body);

            return Ok(new { message = "OTP code sent to your email" });
        }

        [HttpPost("verify-reset-code")]
        public IActionResult VerifyResetCode([FromBody] VerifyOtpDto model)
        {
            if (OtpMemoryStore.OtpCodes.TryGetValue(model.Email, out var otpData))
            {
                if (DateTime.UtcNow > otpData.expiration)
                    return BadRequest("OTP has expired");

                if (otpData.code == model.Code)
                    return Ok(new { message = "OTP verified successfully" });
                else
                    return BadRequest("Incorrect OTP");
            }

            return BadRequest("No OTP found for this email");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                var errorMessages = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                return BadRequest(new { errors = errorMessages });
            }

            return Ok(new { message = "Password has been reset successfully" });
        }

    }
}
