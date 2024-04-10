using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Data
{
    public class Exam
    {
        public int ExamId { get; set; }

        [MaxLength (50)]
        public required string Title { get; set; }
        public int Credits { get; set; }
        public int CourseId { get; set; }
        public int ProfessorId { get; set; }
        public Course Course { get; set; }
        public Professor Professor { get; set; }
        public List<ExamSession>? Sessions { get; set; }
    }
}
