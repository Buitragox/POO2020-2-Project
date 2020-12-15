using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class User
    {
        [Key]
        public long ID { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public string AccountNumber { get; set; }
    }
}