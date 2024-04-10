using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using UniversityManagement.Constants;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorController : ControllerBase
    {
        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<ProfessorController> _logger;
        private readonly ControllersConstants _constants;

        public ProfessorController(UniversityDbContext ctx, Mapper mapper, 
            ILogger<ProfessorController> logger, ControllersConstants constants)
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
                var professors = _ctx.Professors.ToList();

                return Ok(professors.ConvertAll(_mapper.MapEntityToGeneralModel));
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
                var professor = _ctx.Professors
                    .Include(p => p.Exams)
                    .SingleOrDefault(p => p.ProfessorId == id);

                if (professor == null)
                    return BadRequest(_constants.ProfessorNotFound);

                return Ok(_mapper.MapEntityToDetailModel(professor));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("GetFutureExamSession/{id:min(1):int}")]
        public IActionResult GetFutureExamSession([FromRoute] int id)
        {
            try
            {
                var professor = _ctx.Professors
                    .Include(p => p.Exams).ThenInclude(e => e.Course)
                    .Include(p => p.Exams).ThenInclude(e => e.Sessions)
                    .SingleOrDefault(p => p.ProfessorId == id);

                if (professor == null)
                    return BadRequest(_constants.ProfessorNotFound);

                var model = _mapper.MapEntityToProfessorForFutureSessionModel(professor);

                //var examList = model.Exams?.FindAll(e => e.Sessions != null && 
                //    e.Sessions.Any(s => s.DtSession > DateTime.Now));

                var examSessions = model.Exams?.SelectMany(e => e.Sessions.Where(s => s.DtSession > DateTime.Now)).ToList();

                return Ok(examSessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("Create")]
        public IActionResult Create([FromBody] ProfessorDetailModel professor)
        {
            try
            {
                _ctx.Add(_mapper.MapDetailModelToEntity(professor));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.ProfessorNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("Edit")]
        public IActionResult Edit([FromBody] ProfessorDetailModel professor)
        {
            try
            {
                var entity = _ctx.Professors.SingleOrDefault(p => p.ProfessorId == professor.ProfessorId);

                if (entity == null)
                    return BadRequest(_constants.ProfessorNotFound);

                entity.Name = professor.Name;
                entity.Surname = professor.Surname;
                entity.BirthDate = professor.BirthDate;
                entity.FC = professor.FC;

                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.ProfessorNotEdited);
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
                var entity = _ctx.Professors.SingleOrDefault(p => p.ProfessorId == id);

                if (entity == null)
                    return BadRequest(_constants.ProfessorNotFound);

                _ctx.Professors.Remove(entity);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.ProfessorNotDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
