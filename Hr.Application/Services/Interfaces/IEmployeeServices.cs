using Hr.Application.DTOs;
using Hr.Application.DTOs.Employee;
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
        #region Employee Attendance 
        IEnumerable<GetAllEmployeAttendanceDto> GetAllEmployeeAttendance();
        GetAllEmployeAttendanceDto GetAttendanceById(string id);

        #endregion


        #region Employee
        IEnumerable<GetAllEmployeeDto> GetAllEmployee();
        GetAllEmployeeDto GetEmployeetId(string id);
        void CreateEmploye(GetAllEmployeeDto EmployeeDto);
        void UpdateEmploye(GetAllEmployeeDto EmployeeDto, string id);
        bool CheckEmployeeExists(GetAllEmployeeDto EmployeeDto);
        void Remove(string id);

        #endregion
    }
}
