using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityManagement.Constants;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {

        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<ExamController> _logger;
        private readonly ControllersConstants _constants;

        public ExamController(UniversityDbContext ctx, Mapper mapper,
            ILogger<ExamController> logger, ControllersConstants constants)
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
                    var exams = _ctx.Exams
                    .Include(s => s.Course)
                    .Include(s => s.Professor)
                    .Include(s => s.Sessions)
                    .ToList();

                
                return Ok(exams.ConvertAll(_mapper.MapEntityToGeneralModel));
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
                var exams = _ctx.Exams
                    .Include(e => e.Course).ThenInclude(e => e.Students)
                    .Include(e => e.Professor)
                    .Include(e => e.Sessions).ThenInclude(e => e.Grades)
                    .SingleOrDefault(e => e.ExamId == id);

                if (exams == null)
                    return BadRequest(_constants.ExamNotFound);

                return Ok(_mapper.MapEntityToDetailModel(exams));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("GetExamStudents/{id:min(1):int}")]
        public IActionResult GetExamStudents([FromRoute] int id)
        {
            try
            {
                var exams = _ctx.Exams
                   .Include(e => e.Sessions).ThenInclude(s => s.Grades).ThenInclude(g => g.Student)
                   .Include(e => e.Course).ThenInclude(c => c.Students)
                   .SingleOrDefault(e => e.ExamId == id);

                if (exams == null)
                    return BadRequest(_constants.ExamNotFound);

                return Ok(_mapper.MapEntityToExamForStudentListModel(exams));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("Create")]
        public IActionResult Create([FromBody] ExamGeneralModel exam)
        {
            try
            {
                var studentsNumber = _ctx.Students.Where(s => s.CourseId == exam.CourseId).ToList().Count;

                if (studentsNumber > 0)
                    return BadRequest(_constants.ExamNotCreatedForStudent);


                exam.ExamId = 0;
                _ctx.Add(_mapper.MapGeneralModelToEntity(exam));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.ExamNotCreated);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("CreateExamSession")]
        public IActionResult CreateExamSession([FromBody] ExamSessionDetailModel examSession)
        {
            try
            {
                examSession.ExamSessionId = 0;
                _ctx.Add(_mapper.MapDetailModelToEntity(examSession));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.ExamSessionNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("CreateStudentGrade")]
        public IActionResult CreateStudentGrade([FromBody] GradeGeneralModel grade)
        {
            try
            {
                grade.Mark = null;
                _ctx.Add(_mapper.MapGeneralModelToEntity(grade));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.GradeNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("CreateMassiveStudentsGrade")]
        public IActionResult CreateMassiveStudentsGrade([FromBody] GradeSubscriptionMassiveModel grade)
        {
            try
            {
                _ctx.AddRange(_mapper.MapGradeSubscriptionMassiveModelToEntities(grade));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.GradeNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("Edit")]
        public IActionResult Edit([FromBody] ExamGeneralModel exam)
        {
            try
            {
                var entity = _ctx.Exams.SingleOrDefault(e => e.ExamId == exam.ExamId);

                if (entity == null)
                    return BadRequest(_constants.ExamNotFound);

                entity.Title = exam.Title;
                entity.Credits = exam.Credits;
                entity.CourseId = exam.CourseId;
                entity.ProfessorId = exam.ProfessorId;

                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.ExamNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("EditSingleGrade")]
        public IActionResult EditSingleGrade([FromBody] GradeGeneralModel grade)
        {
            try
            {
                var entity = _ctx.Grades.SingleOrDefault(g => g.SessionId == grade.SessionId 
                    && g.StudentId == grade.StudentId);

                if (entity == null)
                    return BadRequest(_constants.GradeNotFound);

                entity.Mark = grade.Mark;

                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.GradeNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("EditMassiveGrade")]
        public IActionResult EditMassiveGrade([FromBody] GradeMassiveModel gradeModel)
        {
            try
            {
                List<Grade> editedGrades = new();
                var grades = _mapper.MapGradeMassiveModelToEntities(gradeModel); 
                
                foreach(var grade in grades)
                {
                    var entity = _ctx.Grades
                        .SingleOrDefault(g => g.SessionId == grade.SessionId && g.StudentId == grade.StudentId);

                    if (entity == null)
                        return BadRequest(_constants.GradeNotFound);

                    entity.Mark = grade.Mark;

                    editedGrades.Add(entity);
                }

                _ctx.UpdateRange(editedGrades);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.GradeNotEdited);
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
                var entity = _ctx.Exams.SingleOrDefault(e => e.ExamId == id);

                if (entity == null)
                    return BadRequest(_constants.ExamNotFound);

                var studentsNumber = _ctx.Students.Where(s => s.CourseId == entity.CourseId).ToList().Count;

                if (studentsNumber > 0)
                    return BadRequest(_constants.ExamNotDeletedForStudent);

                _ctx.Exams.Remove(entity);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.ExamNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
