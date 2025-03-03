using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodiesHaven.Data;
using Microsoft.AspNetCore.Authorization;

namespace FoodiesHaven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public FeedbackController(EFCoreDBContext context)
        {
            _context = context;
        }

        // GET: api/Feedback
        // Retrieves all feedback from the database
        [HttpGet]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult<IEnumerable<FeedbackDTO>>> GetFeedbacks()
        {
            // Fetch all feedback and project them to FeedbackDTO
            return await _context.Feedbacks
                .Select(f => new FeedbackDTO
                {
                    ContactId = f.ContactId, // Primary key remains ContactId
                    Name = f.Name,
                    Email = f.Email,
                    Subject = f.Subject,
                    Message = f.Message,
                    CreatedDate = f.CreatedDate,
                    UserId = f.UserId
                })
                .ToListAsync();
        }

        // GET: api/Feedback/{username}
        // Retrieves a specific feedback by username
        [HttpGet("{username}")]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<FeedbackDTO>> GetFeedback(string username)
        {
            var user = await _context.User
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var feedback = await _context.Feedbacks
                .Where(f => f.UserId == user.UserId)
                .FirstOrDefaultAsync();

            if (feedback == null)
            {
                return NotFound();
            }

            var feedbackDTO = new FeedbackDTO
            {
                ContactId = feedback.ContactId, // Primary key remains ContactId
                Name = feedback.Name,
                Email = feedback.Email,
                Subject = feedback.Subject,
                Message = feedback.Message,
                CreatedDate = feedback.CreatedDate,
                UserId = feedback.UserId
            };

            return Ok(feedbackDTO);
        }

        // POST: api/Feedback/{username}
        // Creates a new feedback
        [HttpPost("{username}")]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<FeedbackDTO>> PostFeedback(string username, FeedbackDTO feedbackDTO)
        {
            var user = await _context.User
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the user has at least one order
            var hasOrder = await _context.Order
                .AnyAsync(o => o.UserId == user.UserId); 

            if (!hasOrder)
            {
                return BadRequest("User must have at least one order to give feedback.");
            }

            // Use the user's name and email from the User table
            var feedback = new Feedback
            {
                Name = user.Name, // Fetching name from User table
                Email = user.Email, // Fetching email from User table
                Subject = feedbackDTO.Subject,
                Message = feedbackDTO.Message,
                CreatedDate = DateTime.Now, // Set the current date and time
                UserId = user.UserId
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Return the created feedback
            var createdFeedbackDTO = new FeedbackDTO
            {
                ContactId = feedback.ContactId, // Primary key remains ContactId
                Name = feedback.Name,
                Email = feedback.Email,
                Subject = feedback.Subject,
                Message = feedback.Message,
                CreatedDate = feedback.CreatedDate,
                UserId = feedback.UserId
            };

            return CreatedAtAction("GetFeedback", new { username = user.Username }, createdFeedbackDTO);
        }

       
    }
}