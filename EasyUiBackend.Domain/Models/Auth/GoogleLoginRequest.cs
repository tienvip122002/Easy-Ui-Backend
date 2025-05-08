using System.ComponentModel.DataAnnotations;

namespace EasyUiBackend.Domain.Models.Auth
{
    public class GoogleLoginRequest
    {
        [Required]
        public string GoogleToken { get; set; }
    }
} 