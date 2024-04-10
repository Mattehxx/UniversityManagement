using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class ProfessorDetailModel
    {
        public int ProfessorId { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public required string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        [MaxLength(20)]
        public required string FC { get; set; }
        public List<ExamFromProfessorModel>? Exams { get; set; }
    }
}
