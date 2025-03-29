using Microsoft.AspNetCore.Identity;

namespace easyUiBackend.Domain.Entities
{
	public class ApplicationUser : IdentityUser
	{
		// 
		public string? FullName { get; set; }
	}
}
