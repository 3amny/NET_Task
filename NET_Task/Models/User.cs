using System.ComponentModel.DataAnnotations;

namespace NET_Task.Models
{
    public class User
    {
        [Key] // primary key
        public int UserId { get; set; }  // Primary Key
        public string? Username { get; set; }
        public int Age { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

    }

}
