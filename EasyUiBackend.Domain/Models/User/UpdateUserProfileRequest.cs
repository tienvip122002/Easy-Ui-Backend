using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EasyUiBackend.Domain.Models.User
{
    public class UpdateUserProfileRequest
    {
        [StringLength(100)]
        public string FullName { get; set; }
        
        [StringLength(200)]
        public string Location { get; set; }
        
        [StringLength(1024)]
        public string Bio { get; set; }
        
        [StringLength(255)]
        [Url]
        public string Website { get; set; }
        
        [EmailAddress]
        public string WorkDisplayEmail { get; set; }
        
        [Phone]
        public string PhoneNumber { get; set; }
        
        // Work history và education
        public List<WorkHistoryItem> WorkHistory { get; set; } = new List<WorkHistoryItem>();
        public List<EducationItem> Education { get; set; } = new List<EducationItem>();
    }
} 