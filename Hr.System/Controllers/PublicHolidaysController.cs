using Hr.Application.DTOs;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicHolidaysController : ControllerBase
    {
        IPublicHolidaysService publicHolidaysService;
        public PublicHolidaysController(IPublicHolidaysService publicHolidaysService)
        {
            this.publicHolidaysService = publicHolidaysService;
        }
        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                var publicHolidays = publicHolidaysService.GetAllPublicHolidays();
                if (publicHolidays != null)
                {
                    var publicHolidaysDTOs = publicHolidays.Select(x => new PublicHolidaysDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Date = x.Day
                    }).ToList();

                    return Ok(publicHolidaysDTOs);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            try
            {
                PublicHolidays publicHoliday = publicHolidaysService.GetPublicHolidayId(id);
                if (publicHoliday != null)
                {
                    var publicHolidayDTO = new PublicHolidaysDTO
                    {
                        Id = publicHoliday.Id,
                        Name = publicHoliday.Name,
                        Date = publicHoliday.Day
                    };
                    return Ok(publicHolidayDTO);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult Create(PublicHolidaysDTO publicHolidayDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!publicHolidaysService.CheckPublicHolidaysExists(publicHolidayDTO))
                {
                    var publicHoliday = new PublicHolidays()
                    {
                        Id = publicHolidayDTO.Id,
                        Name = publicHolidayDTO.Name,
                        Day = publicHolidayDTO.Date,
                        
                    };
                    publicHolidaysService.Create(publicHoliday);
                    return Created("created", publicHoliday);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, PublicHolidaysDTO publicHolidayDTO)
        {
            try
            {
                var existingPublicHoliday = publicHolidaysService.GetPublicHolidayId(id);
                if (existingPublicHoliday == null)
                {
                    return NotFound();
                }
                if (publicHolidayDTO == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (!publicHolidaysService.GetAllPublicHolidays().Any(x => x.Name.ToLower() == publicHolidayDTO.Name.ToLower() && x.Id != id))
                {
                    existingPublicHoliday.Name = publicHolidayDTO.Name;
                    existingPublicHoliday.Day = publicHolidayDTO.Date;
                    publicHolidaysService.Update(existingPublicHoliday);
                    return Ok(publicHolidayDTO);
                }
                return BadRequest(publicHolidayDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var publicHoliday = publicHolidaysService.GetPublicHolidayId(id);
                if (publicHoliday != null)
                {
                    publicHolidaysService.Remove(publicHoliday);
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

    }
}
