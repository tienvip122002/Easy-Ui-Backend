using Microsoft.AspNetCore.Identity;

namespace EasyUiBackend.Domain.Entities
{
	public class ApplicationUser : IdentityUser
	{
		// 
		public string? FullName { get; set; }
	}
}
