﻿using Microsoft.AspNetCore.Identity;

namespace Server.Models
{
  // Add profile data for application users by adding properties to the ApplicationUser class
  public class ApplicationUser : IdentityUser<string> { }
}
