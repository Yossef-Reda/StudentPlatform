using System.ComponentModel.DataAnnotations;

namespace StudentPlatform.Models
{
    public class Lecture
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان المحاضرة مطلوب")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? FilePath { get; set; } 
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}