using Hr.Application.DTOs;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class attendanceController : ControllerBase
    {
        private readonly IAttendanceServices attendanceServices;

        public attendanceController(IAttendanceServices attendanceServices) 
        {
            this.attendanceServices = attendanceServices;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var attendanceDtos = attendanceServices.GetAllAttendance();

                return Ok(attendanceDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }

        }




        [HttpPost]
        public IActionResult Create(AttendanceEmployeDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null || model.LeaveTime < model.ArrivalTime || model.LeaveTime == model.ArrivalTime)
                    {
                        ModelState.AddModelError("LeaveTime", "Leave time cannot be before or equal to arrival time.");
                        return BadRequest(ModelState);
                    }
                    var attendanceDto = new AttendanceEmployeDto
                    {
                        ArrivalTime = model.ArrivalTime,
                        LeaveTime = model.LeaveTime,
                        Date = model.Date,
                        SelectedEmployee = model.SelectedEmployee
                    };

                    attendanceServices.CreateAttendance(attendanceDto);

                    return Ok("Attendance record created successfully.");
                }
                else
                {
                   return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("{id}")]
        public IActionResult Update(AttendanceEmployeDto model, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    attendanceServices.UpdateAttendance(model, id);
                    return Ok("Attendance record updated successfully.");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                bool deleted = attendanceServices.DeleteAttendance(id);

                if (deleted)
                {
                    return Ok("Attendance record deleted successfully.");
                }
                else
                {
                    return NotFound("Attendance record not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }



        [HttpGet("calculate-bonus-discount-hours")]
        public IActionResult CalculateBonusAndDiscountHours(int employeeId, int month)
        {
            var (bonusHours, discountHours) = attendanceServices.CalculateBonusAndDiscountHours(employeeId, month);
            return Ok(new { BonusHours = bonusHours, DiscountHours = discountHours });
        }

    }
}
