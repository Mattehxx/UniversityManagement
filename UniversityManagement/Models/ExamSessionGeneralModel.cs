using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class ExamSessionGeneralModel
    {
        public int ExamSessionId { get; set; }
        public DateTime DtSession { get; set; }
        [MaxLength(200)]
        public required string Location { get; set; }
        public int ExamId { get; set; }
        public string? ExamTitle { get; set; }
        public int? ExamCredits { get; set; }
    }
}
