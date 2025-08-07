using Microsoft.AspNetCore.Identity;

namespace Budget_Buddy_App.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
