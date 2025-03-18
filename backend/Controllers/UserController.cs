using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FoodiesHaven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public UserController(EFCoreDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult<IEnumerable<UsersDTO>> GetUsers()
        {
            var users = _context.User.ToList();
            var userDTOs = users.Select(user => new UsersDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Username = user.Username,
                Mobile = user.Mobile,
                Email = user.Email,
                Address = user.Address,
                PinCode = user.PinCode,
                Role = user.role,
                CreatedDate = user.CreatedDate
            }).ToList();

            return Ok(userDTOs);
        }

        [HttpGet("{username}")]
        [Authorize(Roles = "admin,user")]
        public ActionResult<UsersDTO> GetUserByUsername(string username)
        {
            var user = _context.User.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            var userDTO = new UsersDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Username = user.Username,
                Mobile = user.Mobile,
                Email = user.Email,
                Address = user.Address,
                PinCode = user.PinCode,
                Role = user.role,
                CreatedDate = user.CreatedDate
            };

            return Ok(userDTO);
        }

        [HttpPut("{username}")]
        [Authorize(Roles = "user")]
        public ActionResult UpdateUser(string username, UsersDTO userDTO)
        {
            var user = _context.User.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = userDTO.Name;
            user.Mobile = userDTO.Mobile;
            user.Email = userDTO.Email;
            user.Address = userDTO.Address;
            user.PinCode = userDTO.PinCode;
            user.Password = userDTO.Password;
            user.role = userDTO.Role;

            _context.SaveChanges();

            return NoContent();
        }
    }
}

