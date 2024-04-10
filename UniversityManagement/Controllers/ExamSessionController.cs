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
    public class ExamSessionController : ControllerBase
    {
        private readonly UniversityDbContext _ctx;
        private readonly Mapper _mapper;
        private readonly ILogger<ExamSessionController> _logger;
        private readonly ControllersConstants _constants;

        public ExamSessionController(UniversityDbContext ctx, Mapper mapper, 
            ILogger<ExamSessionController> logger, ControllersConstants constants)
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
                var examSessions = _ctx.ExamSessions
                    .Include(es => es.Exam)
                    .ToList();

                return Ok(examSessions.ConvertAll(_mapper.MapEntityToGeneralModel));
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
                var examSession = _ctx.ExamSessions
                    .Include(es => es.Exam)
                    .Include(es => es.Grades).ThenInclude(g => g.Student)
                    .SingleOrDefault(es => es.ExamSessionId == id);

                if (examSession == null)
                    return BadRequest(_constants.ExamSessionNotFound);

                return Ok(_mapper.MapEntityToDetailModel(examSession));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost, Route("Create")]
        public IActionResult Create([FromBody] ExamSessionDetailModel examSession)
        {
            try
            {
                _ctx.Add(_mapper.MapDetailModelToEntity(examSession));
                return _ctx.SaveChanges() > 0 ? StatusCode(StatusCodes.Status201Created) : BadRequest(_constants.ExamSessionNotCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, Route("Edit")]
        public IActionResult Edit([FromBody] ExamSessionDetailModel examSession)
        {
            try
            {
                var entity = _ctx.ExamSessions.SingleOrDefault(es => es.ExamSessionId == examSession.ExamSessionId);

                if (entity == null)
                    return BadRequest(_constants.ExamSessionNotFound);

                entity.DtSession = examSession.DtSession;
                entity.Location = examSession.Location;
                entity.ExamId = examSession.ExamId;

                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.ExamSessionNotEdited);
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
                var entity = _ctx.ExamSessions.SingleOrDefault(es => es.ExamSessionId == id);

                if (entity == null)
                    return BadRequest(_constants.ExamSessionNotFound);

                var grades = _ctx.Grades.Where(g => g.SessionId == id).ToList();

                if(grades != null && grades.Count > 0)
                {
                    _ctx.Grades.RemoveRange(grades);
                    _ctx.SaveChanges();
                }

                _ctx.ExamSessions.Remove(entity);
                return _ctx.SaveChanges() > 0 ? Ok() : BadRequest(_constants.ExamSessionNotDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
