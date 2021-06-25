using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserIdentityMVC.Models
{
	public class ApplicationUser: IdentityUser
	{ 
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int  UserNameChangeLimit { get; set; }
		public byte[] ProfilePicture { get; set; }
	}

}
