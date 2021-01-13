using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace issue_tracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        ICollection<Issue> Issues { get; set; }
    }
    public enum Priority
    {
        low, medium, high, critical
    }
    
    public enum Phase
    {
        todo, inprogress, done
    }
    public class Issue
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority? Priority { get; set; }
        public Phase? Phase { get; set; }
        public DateTime AddDate { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public int Reviewer { get; set; }   
    }
}

