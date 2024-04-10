using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using UniversityManagement.Constants;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<ExportController> _logger;
        private readonly ControllersConstants _constants;
        private readonly IConfiguration _configuration;
        private readonly string _exportPath;
        private readonly string _SAConnectionString;
        private readonly string _SAExamsContainer;
        private readonly string _SAProfessorsContainer;
        private readonly string _SAStudentsContainer;

        public ExportController(UniversityDbContext ctx, Mapper mapper, 
            ILogger<ExportController> logger, ControllersConstants constants, IConfiguration configuration)
        {
            ConfigurationBuilder config = new ConfigurationBuilder();
            _ctx = ctx;
            _mapper = mapper;
            _logger = logger;
            _constants = constants;
            _configuration = configuration;
            _exportPath = _configuration.GetValue<string>("Export:JsonPath");
            _SAConnectionString = _configuration.GetValue<string>("ConnectionStrings:AzureSA");
            _SAExamsContainer = _configuration.GetValue<string>("Export:Containers:Exams");
            _SAProfessorsContainer = _configuration.GetValue<string>("Export:Containers:Professors");
            _SAStudentsContainer = _configuration.GetValue<string>("Export:Containers:Students");
        }

        [HttpGet, Route("GetAverageGradeByStudent/{courseId:min(1):int}")]
        public async Task<IActionResult> GetAverageGradeByStudent(int courseId)
        {
            try
            {
                var students = _ctx.Students
                    .Include(s => s.Grades).ThenInclude(g => g.Session).ThenInclude(s => s.Exam)
                    .Where(s => s.CourseId == courseId)
                    .ToList();

                List<StudentExportModel> exportModel = students.ConvertAll(_mapper.MapEntityToStudentExportModel);

                string fileName = $"Students_{GetFormattedDateForFile()}.json";
                string completePath = Path.Combine(_exportPath, fileName);

                System.IO.File.WriteAllText(completePath, JsonSerializer.Serialize(exportModel));

                return await ExportToBlob(_SAStudentsContainer, fileName, completePath) ?
                    Ok(exportModel) :
                    StatusCode(StatusCodes.Status500InternalServerError);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("GetAverageGradeByExam/{courseId:min(1):int}")]
        public async Task<IActionResult> GetAverageGradeByExam(int courseId)
        {
            try
            {

                var exams = _ctx.Exams
                    .Include(e => e.Course)
                    .Include(e => e.Sessions).ThenInclude(s => s.Grades)
                    .Where(e => e.CourseId == courseId)
                    .ToList();

                List<ExamExportModel> exportModel = exams.ConvertAll(_mapper.MapEntityToExamExportModel);
                
                string fileName = $"Exams_{GetFormattedDateForFile()}.json";
                string completePath = Path.Combine(_exportPath, fileName);

                System.IO.File.WriteAllText(completePath, JsonSerializer.Serialize(exportModel));

                return await ExportToBlob(_SAExamsContainer, fileName, completePath) ?
                    Ok(exportModel) :
                    StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        [HttpGet, Route("GetAveragePromotedStudentByExam/{courseId:min(1):int}")]
        public async Task<IActionResult> GetAveragePromotedStudentByExam(int courseId)
        {
            try
            {
                var professors = _ctx.Professors
                    .Include(e => e.Exams).ThenInclude(c => c.Course).ThenInclude(s => s.Students).ThenInclude(g => g.Grades)
                    .Include(p => p.Exams).ThenInclude(e => e.Sessions)
                    .Where(e => e.Exams.Any(c => c.CourseId == courseId)).ToList();

                List<ProfessorExportModel> exportModel = professors.ConvertAll(_mapper.MapEntityToProfessorExportModel);

                string fileName = $"Trainers_{GetFormattedDateForFile()}.json";
                string completePath = Path.Combine(_exportPath, fileName);

                System.IO.File.WriteAllText(completePath, JsonSerializer.Serialize(exportModel));

                return await ExportToBlob(_SAProfessorsContainer, fileName, completePath) ? 
                    Ok(exportModel) : 
                    StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private string GetFormattedDateForFile()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace('-', '_').Replace(' ', '_').Replace(':', '_');
        }

        private async Task<bool> ExportToBlob(string container, string fileName, string completePath)
        {
            try
            {
                BlobContainerClient blobContainerClient = new(_SAConnectionString, container);

                BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

                await blobClient.UploadAsync(completePath, false);

                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
    }
}
