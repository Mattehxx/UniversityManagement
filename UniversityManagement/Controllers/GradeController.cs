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
    public class GradeController : ControllerBase
    {

        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<GradeController> _logger;
        private readonly ControllersConstants _constants;

        public GradeController(UniversityDbContext ctx, Mapper mapper,
            ILogger<GradeController> logger, ControllersConstants constants)
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
                var grades = _ctx.Grades.ToList();
                
                return Ok(grades.ConvertAll(_mapper.MapEntityToGeneralModel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("GetSingle/{idStudent:min(1):int}/{idSession:min(1):int}")]
        public IActionResult GetSingle([FromRoute] int idStudent, int idSession)
        {
            try
            {
                var grade = _ctx.Grades.SingleOrDefault(g => g.StudentId == idStudent &&
                    g.SessionId == idSession);

                if (grade == null)
                    return BadRequest(_constants.GradeNotFound);

                return Ok(_mapper.MapEntityToGeneralModel(grade));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("Create")]
        public IActionResult Create([FromBody] GradeGeneralModel grade)
        {
            try
            {
                _ctx.Add(_mapper.MapGeneralModelToEntity(grade));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.GradeNotCreated);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("Edit")]
        public IActionResult Edit([FromBody] GradeGeneralModel grade)
        {
            try
            {
                var entity = _ctx.Grades.SingleOrDefault(g => g.StudentId == grade.StudentId && 
                    g.SessionId == grade.SessionId);

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


        [HttpDelete, Route("Delete/{idStudent:min(1):int}/{idSession:min(1):int}")]
        public IActionResult Delete([FromRoute] int idStudent, int idSession)
        {
            try
            {
                var entity = _ctx.Grades.SingleOrDefault(g => g.StudentId == idStudent &&
                    g.SessionId == idSession);

                if (entity == null)
                    return BadRequest(_constants.GradeNotFound);

                _ctx.Grades.Remove(entity);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.GradeNotEdited);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
