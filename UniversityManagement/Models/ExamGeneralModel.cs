using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class ExamGeneralModel
    {
        public int ExamId { get; set; }

        [MaxLength(50)]
        public required string Title { get; set; }
        public int Credits { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int ProfessorId { get; set; }
        public string? ProfessorName { get; set; }
        public string? ProfessorSurname { get; set; }
    }
}
