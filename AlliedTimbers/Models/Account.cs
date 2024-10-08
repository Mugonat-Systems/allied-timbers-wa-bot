using System;

namespace AlliedTimbers.Models
{
    public class Account
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string Email { get; set; }

        public string Region { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}