using Hr.Application.DTO;
using Hr.Application.DTO.Employee;
using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.Interfaces
{
    public interface IEmployeeServices
    {
        IEnumerable<GetAllEmployeDto> GetAllEmployee();
        GetAllEmployeDto GetById(string id);
    }
}
