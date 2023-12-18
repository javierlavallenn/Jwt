using Application.Config;
using Application.Entities;
using Application.Interface;
using Application.Models;
using Application.Models.Request;
using Application.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IUserService _service;

        public AuthenticationController(IOptionsMonitor<JwtConfig> options, IUserService service)
        {
            _jwtConfig = options.CurrentValue;
            _service = service;
        }


        [HttpPost]
        [Route("SingUp")]
        public async Task<IActionResult> SignUp(SingUpRequest req)
        {
            User entity = await _service.FindByEmailAsync(req.Email);

            if (entity is not null)
                return BadRequest("This email already exists");

            string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(req.Password, 10);

            User user = new()
            {
                Name = req.Name,
                Email = req.Email,
                Password = hash,
                CreateAt = DateTime.Now,
            };

            if (!await _service.CreateAsync(user))
                return BadRequest("Error creating user, please try again");

            string token = GenerateJwtToken(user);

            return StatusCode(201, new Result<AuthResponse>()
            {
                Failure = false,
                Message = "Succes",
                Data = new AuthResponse()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CreateAt = DateTime.Now,
                    Token = token
                }
            });
        }

        [HttpPost]
        [Route("SingIn")]
        public async Task<IActionResult> SingIn(SignInRequest req)
        {
            User? user = await _service.FindByEmailAsync(req.Email);

            if (user is null || user is not null && !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, user.Password))
                return BadRequest("Incorrect credentials, please try again");

            string token = GenerateJwtToken(user);

            return Ok(new Result<AuthResponse>()
            {
                Failure = false,
                Message = "Succses",
                Data = new AuthResponse()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CreateAt = DateTime.Now,
                    Token = token
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();

            byte[] key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

            SecurityTokenDescriptor descriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(descriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
