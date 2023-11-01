using Hr.Application.DTOs.Department;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            this.departmentService = departmentService;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                var Department = departmentService.GetAllDepartment();

                return Ok(Department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                departmentService.GetDepartmentId(id);
                return Ok("Department is founded");
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult Create([FromBody] DepartmentDTO departmentDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (departmentService.CheckDepartmentExists(departmentDTO))
                    {
                        ModelState.AddModelError("DeptName", "Deptartment Name is founded ");
                        return BadRequest(ModelState);
                    }
                    if (ModelState.IsValid)
                    {
                        departmentService.Create(departmentDTO);
                        return Ok("Department Added  Successfully");
                    }
                    return BadRequest("Invalid department data");
                }
                else
                {

                    return BadRequest("Invalid department data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }




        [HttpPut("{id}")]
        public ActionResult Edit(int id, [FromBody] DepartmentDTO updatedDepartmentDTO)
        {
            try
            {
                if (departmentService.CheckDepartmentExists(updatedDepartmentDTO))
                {
                    ModelState.AddModelError("DeptName", "Deptartment Name is founded ");
                    return BadRequest(ModelState);
                }
                if (ModelState.IsValid)
                {
                    departmentService.Update(updatedDepartmentDTO);
                    return Ok("Department Updated  Successfully");
                }

                return BadRequest("Invalid department data");
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                 departmentService.Remove(id);
                return Ok(" Department Has Delete");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }


    }
}
