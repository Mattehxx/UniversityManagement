using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class StudentDetailModel
    {
        public int StudentId { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public required string Surname { get; set; }
        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public required string FC { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public DateTime? CourseStartYear { get; set; }
        public bool? Type {  get; set; }
        public List<GradesFromStudentModel>? Grades { get; set; }

    }
}
