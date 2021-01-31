using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace issue_tracker.Models
{
    public enum Priority
    {
        [Display(Name = "Low")]
        low, 
        [Display(Name = "Medium")]
        medium, 
        [Display(Name = "High")]
        high, 
        [Display(Name = "Critical")]
        critical
    }
    
    public enum Phase
    {
        todo, inprogress, done
    }
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Issue> Issues { get; set; }
    }
    public class Issue
    {

        public int ID { get; set; }
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority? Priority { get; set; }
        public Phase? Phase { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")] 
        public DateTime AddDate { get; set; }
    }
}

