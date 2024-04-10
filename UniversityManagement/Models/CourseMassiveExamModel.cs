namespace UniversityManagement.Models
{
    public class CourseMassiveExamModel
    {
        public int CourseId { get; set; }
        public List<ExamFromCourseModel> Exams { get; set; }
    }
}
