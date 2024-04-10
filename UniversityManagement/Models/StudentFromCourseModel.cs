using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class StudentFromCourseModel
    {
        public int StudentId { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public required string Surname { get; set; }
        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public required string FC { get; set; }
    }
}
