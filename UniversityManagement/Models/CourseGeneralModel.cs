using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class CourseGeneralModel
    {
        public int CourseId { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; }
        public DateTime StartYear { get; set; }
        public bool Type { get; set; }
        public int StudentNumber { get; set; }
        public int ExamNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
