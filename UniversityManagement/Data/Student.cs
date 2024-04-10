using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Data
{
    [Index(nameof(FC), IsUnique = true)]
    public class Student
    {
        public int StudentId { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public required string Surname { get; set; }
        public DateTime BirthDate { get; set; }

        [MaxLength(20), RegularExpression("^[a-zA-Z]{6}[0-9]{2}[a-zA-Z][0-9]{2}[a-zA-Z][0-9]{3}[a-zA-Z]$")]
        public required string FC { get; set; }
        public int CourseId { get; set;}

        public Course Course { get; set; }

        public List<Grade>? Grades { get; set;}
    }
}
