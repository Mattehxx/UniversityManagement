using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class ProfessorGeneralModel
    {
        public int ProfessorId { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public required string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public required string FC { get; set; }
    }
}
