using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Data
{
    public class ExamSession
    {
        public int ExamSessionId {  get; set; }
        public DateTime DtSession {  get; set; }
        [MaxLength(200)]
        public required string Location { get; set; }
        public int ExamId {  get; set; }
        public Exam Exam { get; set; }  
        public List<Grade>? Grades { get; set; }
    }
}
