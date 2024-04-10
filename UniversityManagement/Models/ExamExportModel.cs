using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class ExamExportModel
    {
        public int ExamId { get; set; }

        [MaxLength(50)]
        public required string Title { get; set; }
        public int Credits { get; set; }
        public double? AverageMark {  get; set; }
    }
}
