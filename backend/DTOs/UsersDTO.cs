
namespace FoodiesHaven.DTOs
{
    public class UsersDTO
    {
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public ICollection<string> Address { get; set; }
        public string PinCode { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}