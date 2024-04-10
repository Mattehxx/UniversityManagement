using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using UniversityManagement.Constants;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<CourseController> _logger;
        private readonly ControllersConstants _constants;

        public CourseController(UniversityDbContext ctx, Mapper mapper, 
            ILogger<CourseController> logger, ControllersConstants constants)
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
                var courses = _ctx.Courses
                    .Include(c => c.Students)
                    .Include(c => c.Exams)
                    .ToList();

                return Ok(courses.ConvertAll(_mapper.MapEntityToGeneralModel));
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
                var course = _ctx.Courses
                    .Include(c => c.Students)
                    .Include(c => c.Exams).ThenInclude(e => e.Professor)
                    .SingleOrDefault(c => c.CourseId == id);

                if (course == null)
                    return BadRequest(_constants.CourseNotFound);

                return Ok(_mapper.MapEntityToDetailModel(course));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("Create")]
        public IActionResult Create([FromBody] CourseDetailModel course)
        {
            try
            {
                _ctx.Add(_mapper.MapDetailModelToEntity(course));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.CourseNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("CreateStudent")]
        public IActionResult CreateStudent(StudentGeneralModel student)
        {
            try
            {
                _ctx.Add(_mapper.MapGeneralModelToEntity(student));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.StudentNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("CreateExam")]
        public IActionResult CreateExam(ExamGeneralModel exam)
        {
            try
            {
                var studentsNumber = _ctx.Students.Where(s => s.CourseId == exam.CourseId).ToList().Count;

                if (studentsNumber > 0)
                    return BadRequest(_constants.ExamNotCreatedForStudent);

                _ctx.Add(_mapper.MapGeneralModelToEntity(exam));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.ExamNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("CreateMassiveExams")]
        public IActionResult CreateMassiveExams(MassiveExamsModel model)
        {
            try
            {
                var courseModel = ReadMassiveExamCSVFile(model);

                var studentsNumber = _ctx.Students.Where(s => s.CourseId == courseModel.CourseId).ToList().Count;

                if (studentsNumber > 0)
                    return BadRequest(_constants.ExamNotCreatedForStudent);

                _ctx.AddRange(_mapper.MapCourseMassiveExamModelToEntities(courseModel));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.ExamNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("Edit")]
        public IActionResult Edit([FromBody] CourseDetailModel course)
        {
            try
            {
                var entity = _ctx.Courses.SingleOrDefault(c => c.CourseId == course.CourseId);

                if (entity == null)
                    return BadRequest(_constants.CourseNotFound);

                entity.Title = course.Title;
                entity.StartYear = course.StartYear;
                entity.Type = course.Type;

                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.CourseNotEdited);
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
                var entity = _ctx.Courses.SingleOrDefault(c => c.CourseId == id);

                if (entity == null)
                    return BadRequest(_constants.CourseNotFound);

                _ctx.Courses.Remove(entity);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.CourseNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete, Route("DeleteExam/{id:min(1):int}")]
        public IActionResult DeleteExam([FromRoute] int id)
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
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.CourseNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private CourseMassiveExamModel ReadMassiveExamCSVFile (MassiveExamsModel model)
        {
            List<string?> fileText = new();
            using (var reader = new StreamReader(model.File.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    fileText.Add(reader.ReadLine());
            }

            List<ExamFromCourseModel> examModels = new();
            int i = 0;

            foreach (var line in fileText)
            {
                if (i == 0 || line == null)
                {
                    i++;
                    continue;
                }

                var splitted = line.Split(',');
                string title = splitted[0].Replace('"', ' ').Replace('\\', ' ').Trim();
                int credits = int.Parse(splitted[1]);
                string fc = splitted[2].Replace('"', ' ').Replace('\\', ' ').Trim();

                var professor = _ctx.Professors.FirstOrDefault(p => p.FC == fc);

                if (professor != null)
                {
                    examModels.Add(new ExamFromCourseModel()
                    {
                        ExamId = 0,
                        Title = title,
                        Credits = credits,
                        ProfessorId = professor.ProfessorId
                    });
                }
            }

            return new CourseMassiveExamModel()
            {
                CourseId = model.CourseId,
                Exams = examModels
            };
        }
    }
}
