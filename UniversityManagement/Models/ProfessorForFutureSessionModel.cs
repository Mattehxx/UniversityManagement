using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class ProfessorForFutureSessionModel
    {
        public int ProfessorId { get; set; }
        public List<ExamForFutureSessionModel>? Exams { get; set; }
    }
}
