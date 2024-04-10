namespace UniversityManagement.Models
{
    public class GradeMassiveModel
    {
        public int SessionId { get; set; }
        public List<GradeGeneralModel> StudentMarks { get; set; }
    }
}
