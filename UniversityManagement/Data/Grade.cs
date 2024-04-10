using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Data
{
    [PrimaryKey(nameof(StudentId), nameof(SessionId))]
    public class Grade
    {
        public int StudentId {  get; set; }
        public int SessionId { get; set; }
        [Range(18, 31)]
        public int? Mark {  get; set; }
        public Student Student { get; set; }
        public ExamSession Session { get; set; }
    }
}
