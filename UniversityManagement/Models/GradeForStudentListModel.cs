namespace UniversityManagement.Models
{
    public class GradeForStudentListModel
    {
        public int StudentId { get; set; }
        public int SessionId { get; set; }
        public int? Mark {  get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
    }
}
