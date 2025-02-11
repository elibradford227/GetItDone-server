using Microsoft.AspNetCore.Identity;

namespace GetItDone.models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
        public List<UserTask>? Tasks { get; set; } = new List<UserTask>();
    }
}
