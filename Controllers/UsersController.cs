using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.Dto;
using ProductsAPI.Models;
using ProductsAPI.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UsersController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,IConfiguration configuration)
        {
            _usermanager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(UserDto userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser
            {
                UserName = userModel.Username,
                Email = userModel.Email,
                FullName = userModel.FullName,
                CreatedAt = DateTime.Now
            };

            var result = await _usermanager.CreateAsync(user, userModel.Password);

            if (result.Succeeded)
            {
                return StatusCode(201);
            }

            return BadRequest(result.Errors);

        }

        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _usermanager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new {message = "Email bilgisi hatalı."});
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return Ok(new {token = GenerateJWT(user)});
            }

            return BadRequest(new { message = "Girdiğiniz bilgiler hatalı."});
        }

        private object GenerateJWT(AppUser user)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? "")
                }
                ),

                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenhandler.CreateToken(tokenDescriptor);

            return tokenhandler.WriteToken(token);


        }
    }
}