using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NGK_Lab3_V2.Models
{
    public class User : IdentityUser
    {
        // Error here, UserID is always 0
        // Should be unique
        public long UserId{ get; set; }

        [MaxLength(64)]
        public string FirstName{ get; set; }

        [MaxLength(32)]
        public string LastName{ get; set; }

        [MaxLength(254)]
        public string Email { get; set; }

        [MaxLength(60)]
        public string PwHash{ get; set; }
    }

    public class UserRole : IdentityRole
    {
        public enum RoleType
        {
            Reader = 0,
            Poster = 1,
            Administrator = 2,
        }
    }
}