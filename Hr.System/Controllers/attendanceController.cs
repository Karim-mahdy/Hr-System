using Hr.Application.DTOs;
using Hr.Application.DTOs.Employee;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceServices attendanceServices;

        public AttendanceController(IAttendanceServices attendanceServices) 
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
        public IActionResult Create(AttendanceEmployeDto attendanceEmployeDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime dateToCheck = attendanceEmployeDto.Date;
                    string dayOfWeek = attendanceServices.GetDayOfWeekForDate(dateToCheck);

                    List<string> employeeWeekendDays = attendanceServices.GetEmployeeWeekendDays(attendanceEmployeDto.SelectedEmployee);
                    if (employeeWeekendDays.Contains(dayOfWeek))
                    {
                        ModelState.AddModelError("Date", "Attendance on a weekend day is not allowed.");
                        return BadRequest(ModelState);
                    }
                    if (attendanceServices.CheckAttendanceExists(attendanceEmployeDto))
                    {
                        ModelState.AddModelError("Date", "The Employee Has Added Attendance");
                        return BadRequest(ModelState);
                    }
                    var attendanceDto = new AttendanceEmployeDto
                    {
                        ArrivalTime = attendanceEmployeDto.ArrivalTime,
                        LeaveTime = attendanceEmployeDto.LeaveTime,
                        Date = attendanceEmployeDto.Date,
                        SelectedEmployee = attendanceEmployeDto.SelectedEmployee
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
        public IActionResult Update(AttendanceEmployeDto attendanceEmployeDto, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (attendanceEmployeDto == null ||
                      !TimeSpan.TryParse(attendanceEmployeDto.ArrivalTime, out TimeSpan arrivalTime) ||
                      !TimeSpan.TryParse(attendanceEmployeDto.LeaveTime, out TimeSpan leaveTime) ||
                       arrivalTime >= leaveTime)
                    {
                        ModelState.AddModelError("LeaveTime", "Leave time cannot be before or equal to arrival time.");
                        return BadRequest(ModelState);
                    }
                    if (attendanceServices.GetAllAttendance().Any(
                        x => x.Date==attendanceEmployeDto.Date &&
                        x.SelectedEmployee==attendanceEmployeDto.SelectedEmployee &&
                        x.Id != attendanceEmployeDto.Id))
                    {
                        ModelState.AddModelError("Date", "the name is founded ");
                        return BadRequest(ModelState);
                    }
                    attendanceServices.UpdateAttendance(attendanceEmployeDto, id);
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
                    return Ok(new { message = "Attendance record deleted successfully."});
                }
                else
                {
                    return NotFound(new { message = "Attendance record not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }
    }
}
