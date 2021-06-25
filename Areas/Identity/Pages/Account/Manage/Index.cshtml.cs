using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserIdentityMVC.Models;

namespace UserIdentityMVC.Areas.Identity.Pages.Account.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public IndexModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public string Username { get; set; }
		[TempData]
		public string StatusMessage { get; set; }
		[TempData]
		public string UserNameChangeLimitMsg { get; set; }
		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Display(Name = "First Name")]
			public string FirstName { get; set; }
			[Display(Name = "Last Name")]
			public string LastName { get; set; }
			[Display(Name = "User name")]
			public string UserName { get; set; }
			[Display(Name = "Profile picture")]
			public byte[] ProfilePicture { get; set; }
			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
		}

		private async Task LoadAsync(ApplicationUser user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			var firstName = user.FirstName;
			var lastName = user.LastName;
			var profilePicture = user.ProfilePicture;
			Username = userName;
			Input = new InputModel
			{
				PhoneNumber = phoneNumber,
				UserName = userName,
				FirstName = firstName,
				LastName = lastName,
				ProfilePicture = profilePicture
			};
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}
			UserNameChangeLimitMsg = $"You can change your user name {user.UserNameChangeLimit} more time(s).";
			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var firstName = user.FirstName;
			var lastName = user.LastName;
			if (Input.FirstName != firstName)
			{
				user.FirstName = Input.FirstName;
				await _userManager.UpdateAsync(user);
			}

			if (Input.LastName != lastName)
			{
				user.LastName = Input.LastName;
				await _userManager.UpdateAsync(user);		
			}
			
			// keep track user name change limit 
			if (user.UserNameChangeLimit > 0)
			{
				if (Input.UserName != user.UserName)
				{
					// if current user name is existed 
					var existingUserName = await _userManager.FindByNameAsync(Input.UserName);
					if (existingUserName != null)
					{
						StatusMessage = "User name is already take. Select another name.";
						return RedirectToPage();
					}

					var setUserName = await _userManager.SetUserNameAsync(user, Input.UserName);
					if (!setUserName.Succeeded)
					{
						StatusMessage = "Unexpected error when trying to set user name.";
						return RedirectToPage();
					}
					// if changing is valid so decrease change time by 1
					else
					{
						user.UserNameChangeLimit -= 1;
						await _userManager.UpdateAsync(user);
					}
				}
			}

			// working with image file 
			// using memory stream to convert the image file to object/byte array
			if (Request.Form.Files.Count > 0)
			{
				IFormFile file = Request.Form.Files.FirstOrDefault();
				using (var dataStream = new MemoryStream())
				{
					await file.CopyToAsync(dataStream);
					user.ProfilePicture = dataStream.ToArray();
				}
				await _userManager.UpdateAsync(user);
			}

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					StatusMessage = "Unexpected error when trying to set phone number.";
					return RedirectToPage();
				}
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}
