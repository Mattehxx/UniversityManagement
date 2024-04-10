using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class ExamFromProfessorModel
    {
        public int ExamId { get; set; }
        [MaxLength(50)]
        public required string Title { get; set; }
        public int Credits { get; set; }
    }
}
