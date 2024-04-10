using System.Runtime.CompilerServices;

namespace UniversityManagement.Models
{
    public class GradeFromExamSessionModel
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentSurname { get; set; }
        public int? Mark {  get; set; }

    }
}
