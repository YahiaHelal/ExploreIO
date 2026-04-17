using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppRole: IdentityRole<int> // each user can be multiple roles and vice versa ?
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
        
    }
}