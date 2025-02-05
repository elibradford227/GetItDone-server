using Microsoft.AspNetCore.Identity;

namespace GetItDone.models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
    }
}
