using Microsoft.AspNetCore.Identity;

namespace WebApplicationApp.Models
{
    public class ApplicationUser: IdentityUser
    {

        public string FullName { get; set; }

    }
}
