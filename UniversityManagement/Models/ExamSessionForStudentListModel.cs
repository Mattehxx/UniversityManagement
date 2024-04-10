using UniversityManagement.Models;

namespace UniversityManagement.Data
{
    public class ExamSessionForStudentListModel
    {
        public int ExamSessionId { get; set; }
        public List<GradeForStudentListModel>? GradeList { get; set; }
    }
}
