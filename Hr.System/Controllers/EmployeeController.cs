using Hr.Application.DTOs;
using Hr.Application.DTOs.Employee;
using Hr.Application.Interfaces;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeServices employeeServices;

        public EmployeeController(IEmployeeServices employeeServices)
        {

            this.employeeServices = employeeServices;
        }

        [HttpGet]
        public IActionResult GetAllEmploye()
        {
            try
            {
                var EmployeDto = employeeServices.GetAllEmployee();

                return Ok(EmployeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var EmployeDto = employeeServices.GetEmployeeId(id);

                return Ok(EmployeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create(GetAllEmployeeDto EmployeeDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (EmployeeDto == null ||
                        !TimeSpan.TryParse(EmployeeDto.ArrivalTime, out TimeSpan arrivalTime) ||
                        !TimeSpan.TryParse(EmployeeDto.LeaveTime, out TimeSpan leaveTime) ||
                         arrivalTime >= leaveTime)
                    {
                        ModelState.AddModelError("LeaveTime", "Leave time cannot be before or equal to arrival time.");
                        return BadRequest(ModelState);
                    }
                    if (employeeServices.CheckEmployeeExists(EmployeeDto))
                    {
                        ModelState.AddModelError("FirstName", "First Name and Last Name is founded ");
                        return BadRequest(ModelState);
                    }
                    employeeServices.CreateEmployee(EmployeeDto);

                    return Ok("Employe record created successfully.");
                }
                else
                {
                    return BadRequest(EmployeeDto);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(GetAllEmployeeDto employeeDto, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (employeeDto == null ||
                      !TimeSpan.TryParse(employeeDto.ArrivalTime, out TimeSpan arrivalTime) ||
                      !TimeSpan.TryParse(employeeDto.LeaveTime, out TimeSpan leaveTime) ||
                       arrivalTime >= leaveTime)
                    {
                        ModelState.AddModelError("LeaveTime", "Leave time cannot be before or equal to arrival time.");
                        return BadRequest(ModelState);
                    }
                    if (employeeServices.GetAllEmployee().Any(
                        x => x.FirstName.ToLower() == employeeDto.FirstName.ToLower()
                        && x.LastName.ToLower() == employeeDto.LastName.ToLower() &&
                        x.ID != employeeDto.ID))
                    {
                        ModelState.AddModelError("FirstName", "the name is founded plz enter another name");
                        return BadRequest(ModelState);
                    }
                    employeeServices.UpdateEmployee(employeeDto, id);
                    return Ok(employeeDto);
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
                employeeServices.Remove(id);
                return Ok("Employee deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
