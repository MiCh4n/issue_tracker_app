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
        [InverseProperty("ApplicationUserAuthorId")]
        public virtual List<Issue> AuthorRelationsId { get; set; }
        [InverseProperty("ApplicationUserReviewer")]
        public virtual List<Issue> ReviewerRelations { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        [DefaultValue("User")]
        public override string Name { get; set; }
    }

    public class Issue
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }
        
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser ApplicationUserAuthorId { get; set; }
        public string AuthorId { get; set; }
        
        [ForeignKey("ReviewerId")]
        public virtual ApplicationUser ApplicationUserReviewer { get; set; }
        public string ReviewerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority? Priority { get; set; }
        public Phase? Phase { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")] 
        public DateTime AddDate { get; set; }
    }
}

