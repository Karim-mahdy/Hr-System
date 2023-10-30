using Hr.Application.DTO.Employee;
using Hr.Application.Interfaces;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.implementation
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IUniteOfWork uniteOfWork;

        public EmployeeServices(IUniteOfWork uniteOfWork)
        {
            this.uniteOfWork = uniteOfWork;
        }
        public IEnumerable<GetAllEmployeDto> GetAllEmployee()
        {
            var listOfEmployee= new List<GetAllEmployeDto>();
            var employes = uniteOfWork.employeeRepository.GetAll();
            foreach (var employee in employes)
            {
                var emp = new GetAllEmployeDto()
                {
                    ID = employee.Id,
                    Name = employee.FirstName + " " + employee.LastName,
                };
                listOfEmployee.Add(emp);
            }
            return listOfEmployee;
        }

        public GetAllEmployeDto GetById(string id)
        {
            var employee = uniteOfWork.employeeRepository.Get(x => x.Id == id);
            if(employee == null)
            {
                throw new Exception("Not found Employee");
            }
            else
            {
                var empDto = new GetAllEmployeDto()
                {
                    ID = employee.Id,
                    Name = employee.FirstName+" "+ employee.LastName,
                };
                return empDto;
            }
        }
    }
}
