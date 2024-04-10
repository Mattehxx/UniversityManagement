using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class ExamDetailModel
    {
        public int ExamId { get; set; }

        [MaxLength(50)]
        public required string Title { get; set; }
        public int Credits { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public DateTime? CourseStartYear { get; set; }
        public bool? CourseType { get; set; }
        public int ProfessorId { get; set; }
        public string? ProfessorName { get; set; }
        public string? ProfessorSurname { get; set; }
        public List<ExamSessionFromExamModel>? Sessions { get; set; }
    }
}
