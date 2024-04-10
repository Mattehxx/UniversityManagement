using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Data
{
    public class Course
    {
        public int CourseId { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; }
        public DateTime StartYear { get; set; }
        public bool Type { get; set; }
        public List<Student>? Students { get; set; }
        public List<Exam>? Exams { get; set; }
    }
}
