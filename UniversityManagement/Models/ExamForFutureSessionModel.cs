using System.ComponentModel.DataAnnotations;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class ExamForFutureSessionModel
    {
        public int ExamId { get; set; }
        public List<ExamSessionGeneralModel>? Sessions { get; set; }
    }
}
