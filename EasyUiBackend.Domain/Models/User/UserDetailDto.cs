using System;
using System.Collections.Generic;

namespace EasyUiBackend.Domain.Models.User
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }
        public string WorkDisplayEmail { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Work history và education được chuyển từ JSON string sang object
        public List<WorkHistoryItem> WorkHistory { get; set; } = new List<WorkHistoryItem>();
        public List<EducationItem> Education { get; set; } = new List<EducationItem>();
        
        // Follow information
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
    }
    
    public class WorkHistoryItem
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string YearStart { get; set; }
        public string YearEnd { get; set; }
        public string Description { get; set; }
    }
    
    public class EducationItem
    {
        public string Institution { get; set; }
        public string Degree { get; set; }
        public string Field { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string Description { get; set; }
    }
} 