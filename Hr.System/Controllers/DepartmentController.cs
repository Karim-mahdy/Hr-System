using Hr.Application.DTO;
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
            var allDepartments = departmentService.GetAllDepartment();
            if (allDepartments != null && allDepartments.Any())
            {
                var departmentDTOs = allDepartments.Select(department => new DepartmentDTO
                {
                    Id = department.Id,
                    Name = department.DeptName
                }).ToList();

                return Ok(departmentDTOs);
            }
            return NotFound();
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var department = departmentService.GetDepartmentId(id);

            if (department != null)
            {
                var departmentDTO = new DepartmentDTO
                {
                    Id = department.Id,
                    Name = department.DeptName
                };

                return Ok(departmentDTO);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Create([FromBody] DepartmentDTO departmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid department data");
            }

            var department = new Department
            {
                DeptName = departmentDTO.Name
            };

            departmentService.Create(department);

            // Return the created department as a DTO with the generated ID
            var createdDTO = new DepartmentDTO
            {
                Id = department.Id,
                Name = department.DeptName
            };

            return CreatedAtAction("Get", new { id = createdDTO.Id }, createdDTO);
        }

        [HttpPut("{id}")]
        public ActionResult  Edit(int id, [FromBody] DepartmentDTO updatedDepartmentDTO)
        {
            var existingDepartment = departmentService.GetDepartmentId(id);

            if (existingDepartment == null)
            {
                return NotFound();
            }

            if (updatedDepartmentDTO == null)
            {
                return BadRequest("Invalid department data");
            }
             
            existingDepartment.DeptName = updatedDepartmentDTO.Name;
             
            departmentService.Update(existingDepartment);

            // Return the updated  department 
            var updatedDTO = new DepartmentDTO
            {
                Id = existingDepartment.Id,
                Name = existingDepartment.DeptName
            };

            return Ok(updatedDTO);
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var department = departmentService.GetDepartmentId(id);

            if (department == null)
            {
                return NotFound();
            }
            
            departmentService.Remove(department);

            return NoContent();  
        }

        
    }
}
