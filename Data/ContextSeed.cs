using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserIdentityMVC.Models;

namespace UserIdentityMVC.Data
{
	public class ContextSeed
	{
		public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			//Seed Roles
			await roleManager.CreateAsync(new IdentityRole(Enums.Roles.SuperAdmin.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Moderator.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Basic.ToString()));
		}
	   
		public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, 
			RoleManager<IdentityRole> roleManager)
		{
			// seed default user
			var defaultUser = new ApplicationUser
			{
				UserName = "SuperAdmin",
				Email = "superadmin@gmail.com",
				FirstName = "Kane",
				LastName = "Nguyen",
				EmailConfirmed = true,
				PhoneNumberConfirmed = true
			};

			if (userManager.Users.All(u => u.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "123passw0rd@");
					await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Basic.ToString());
					await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Moderator.ToString());
					await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
					await userManager.AddToRoleAsync(defaultUser, Enums.Roles.SuperAdmin.ToString());
				}
			}
		}
	}
}
