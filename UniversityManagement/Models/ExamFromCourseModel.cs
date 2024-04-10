using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class ExamFromCourseModel
    {
        public int ExamId { get; set; }

        [MaxLength(50)]
        public required string Title { get; set; }
        public int Credits { get; set; }
        public int ProfessorId { get; set; }

        [MaxLength(50)]
        public string? ProfessorName { get; set; }

        [MaxLength(50)]
        public string? ProfessorSurname { get; set; }
    }
}
