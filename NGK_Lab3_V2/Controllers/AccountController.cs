using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NGK_Lab3_V2.DTO_s;
using NGK_Lab3_V2.Models;
using static BCrypt.Net.BCrypt;

namespace NGK_Lab3_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        const int BCryptFactor = 11;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DtoUser dtoUser)
        {
            var newUser = new User
            {
                Email = dtoUser.Email,
                UserName = dtoUser.Email,
                FirstName = dtoUser.FirstName,
                LastName = dtoUser.LastName,
                NormalizedEmail = dtoUser.Email.Normalize().ToUpperInvariant(),
                PasswordHash = HashPassword(dtoUser.Password, BCryptFactor)
            };

            var userCreationResult = await _userManager.CreateAsync(newUser, dtoUser.Password);
            if (userCreationResult.Succeeded)
                return Ok(newUser);

            foreach (var error in userCreationResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DtoUser dtoUser)
        {
            var user = await _userManager.FindByNameAsync(dtoUser.Email.Normalize().ToUpperInvariant());
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Email");
                return BadRequest(ModelState);
            }

            var passwordSignInResult = await _signInManager.CheckPasswordSignInAsync(user, dtoUser.Password, false);
            if (passwordSignInResult.Succeeded)
                return new OkObjectResult(GenerateToken(dtoUser.Email));

            return BadRequest("Invalid Password");
        }



        private string GenerateToken(string userName)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Nbf,
                    new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,
                    new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SomeMotherFuckingSecret")),
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}