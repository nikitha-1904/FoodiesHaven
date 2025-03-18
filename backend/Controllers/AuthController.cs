using Microsoft.AspNetCore.Mvc;
using FoodiesHaven.Models;
using FoodiesHaven.DTOs;
//using BCrypt.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Eventhub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(EFCoreDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        

        public async Task<IActionResult> Signup(UsersDTO userDTO)
        {
            // Check if the username or email already exists
            if (await _context.User.AnyAsync(u => u.Username == userDTO.Username || u.Email == userDTO.Email))
            {
                return BadRequest("Username or Email already exists.");
            }

            var user = new Users
            {
                Name = userDTO.Name,
                Username = userDTO.Username,
                Mobile=userDTO.Mobile,
                Password = userDTO.Password,
                Email = userDTO.Email,
                Address = userDTO.Address,
                PinCode = userDTO.PinCode,
                role =userDTO.Role,
                
            };
            

            // Add the user to the database
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            // Find the user by username
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == loginDTO.Username);

            if (user == null || (loginDTO.Password != user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Authentication successful
            return Ok(new { token });
        }

        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSettings:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["jwtSettings:issuer"],
                audience: _configuration["jwtSettings:audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}