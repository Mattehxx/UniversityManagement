namespace UniversityManagement.Models
{
    public class GradeSubscriptionMassiveModel
    {
        public int SessionId { get; set; }
        public List<int> StudentIds { get; set; }
        public int? Mark { get; set; }
    }
}
