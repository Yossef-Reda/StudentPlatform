using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPlatform.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الواجب مطلوب")]
        [Display(Name = "عنوان الواجب")]
        public string Title { get; set; }

        [Display(Name = "تفاصيل المطلوب")]
        public string Description { get; set; }

        [Display(Name = "ملف الواجب (PDF / ملف مرفق)")]
        public string? FilePath { get; set; } 

        [Required(ErrorMessage = "تاريخ التسليم مطلوب")]
        [Display(Name = "الموعد النهائي (Deadline)")]
        public DateTime Deadline { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int LectureId { get; set; }

        [ForeignKey("LectureId")]
        public Lecture Lecture { get; set; }

        public ICollection<StudentSubmission> Submissions { get; set; }
    }
}