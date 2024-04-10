using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class StudentBookletModel
    {
        public int StudentId { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public required string Surname { get; set; }
        public List<GradesFromStudentModel>? Grades { get; set; }
    }
}
