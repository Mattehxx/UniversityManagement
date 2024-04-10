using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class GradeGeneralModel
    {
        public int StudentId { get; set; }
        public int SessionId { get; set; }
        [Range(18, 31)]
        public int? Mark { get; set; }
    }
}
