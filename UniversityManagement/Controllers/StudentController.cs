using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Constants;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<StudentController> _logger;
        private readonly ControllersConstants _constants;

        public StudentController(UniversityDbContext ctx, Mapper mapper,
            ILogger<StudentController> logger, ControllersConstants constants)
        {
            _ctx = ctx;
            _mapper = mapper;
            _logger = logger;
            _constants = constants;
        }

        [HttpGet, Route("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var students = _ctx.Students
                .Include(s => s.Course)
                .Include(s => s.Grades)
                .ToList();


                return Ok(students.ConvertAll(_mapper.MapEntityToGeneralModel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("GetSingle/{id:min(1):int}")]
        public IActionResult GetSingle([FromRoute] int id)
        {
            try
            {
                var student = _ctx.Students
                    .Include(s => s.Course).ThenInclude(s => s.Exams)
                    .Include(s => s.Grades).ThenInclude(s => s.Session).ThenInclude(es => es.Exam)
                    .SingleOrDefault(e => e.StudentId == id);

                if (student == null)
                    return BadRequest(_constants.StudentNotFound);

                return Ok(_mapper.MapEntityToDetailModel(student));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("GetBooklet/{id:min(1):int}")]
        public IActionResult GetBooklet([FromRoute] int id)
        {
            try
            {
                var student = _ctx.Students
                    .Include(s => s.Course).ThenInclude(s => s.Exams)
                    .Include(s => s.Grades).ThenInclude(s => s.Session).ThenInclude(s => s.Exam)
                    .SingleOrDefault(e => e.StudentId == id);

                if (student == null)
                    return BadRequest(_constants.StudentNotFound);

                student.Grades = student.Grades?.FindAll(g => g.Mark != null);

                return Ok(_mapper.MapEntityToStudentBookletModel(student));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpGet, Route("GetMissingExams/{id:min(1):int}")]
        public IActionResult GetMissingExams([FromRoute] int id)
        {
            try
            {
                var student = _ctx.Students
                    .Include(s => s.Course).ThenInclude(s => s.Exams)
                    .Include(s => s.Grades).ThenInclude(s => s.Session).ThenInclude (s => s.Exam)
                    .SingleOrDefault(e => e.StudentId == id);

                if (student == null)
                    return BadRequest(_constants.StudentNotFound);

                var model = _mapper.MapEntityToStudentBookletModel(student);

                var missingExams = model.Grades?.Where(g => g.Mark == null && g.DtSession > DateTime.Now).ToList();

                return Ok(missingExams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("Create")]
        public IActionResult Create([FromBody] StudentDetailModel student)
        {
            try
            {
                _ctx.Add(_mapper.MapDetailModelToEntity(student));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.StudentNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("Edit")]
        public IActionResult Edit([FromBody] StudentDetailModel student)
        {
            try
            {
                var entity = _ctx.Students.SingleOrDefault(e => e.StudentId == student.StudentId);

                if (entity == null)
                    return BadRequest(_constants.StudentNotFound);
                
                entity.Name = student.Name;
                entity.Surname = student.Surname;
                entity.BirthDate = student.BirthDate;
                entity.FC = student.FC;
                entity.CourseId = student.CourseId;

                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.StudentNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete, Route("Delete/{id:min(1):int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var entity = _ctx.Students.SingleOrDefault(e => e.StudentId == id);

                if (entity == null)
                    return BadRequest(_constants.StudentNotFound);

                var grades = _ctx.Grades.Where(g => g.StudentId == id).ToList();

                if (grades != null && grades.Count > 0)
                {
                    _ctx.Grades.RemoveRange(grades);
                    _ctx.SaveChanges();
                }

                _ctx.Students.Remove(entity);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.StudentNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
