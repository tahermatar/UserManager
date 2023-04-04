using Auth.Web.Enums;
using Microsoft.AspNetCore.Identity;
using System;

namespace Auth.Web.Models
{
    public class User : IdentityUser
    {
        public bool IsDelete { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserType UserType { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
