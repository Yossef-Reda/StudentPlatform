using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StudentPlatform.Models
{
    public class StudentSubmission
    {
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Display(Name = "اسم الطالب")]
        public string StudentName { get; set; }

        public string FilePath { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string? Grade { get; set; }
    }
}