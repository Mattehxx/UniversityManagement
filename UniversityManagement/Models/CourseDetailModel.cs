using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class CourseDetailModel
    {
        public int CourseId { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; }
        public DateTime StartYear { get; set; }
        public bool Type { get; set; }
        public bool? IsActive { get; set; }
        public List<StudentFromCourseModel>? Students { get; set; }
        public List<ExamFromCourseModel>? Exams { get; set; }
    }
}
