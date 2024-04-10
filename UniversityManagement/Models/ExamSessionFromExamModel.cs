using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class ExamSessionFromExamModel
    {
        public int ExamSessionId { get; set; }
        public DateTime DtSession { get; set; }
        [MaxLength(200)]
        public required string Location { get; set; }
    }
}
