using Microsoft.AspNetCore.Identity;

namespace ServerWithCustomKey.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationRole : IdentityRole<int>
{
  public ApplicationRole() { }
  public ApplicationRole(string name) { Name = name; }
}
