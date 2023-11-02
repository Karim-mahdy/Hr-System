
using Hr.Application.Common;
using Hr.Application.DTOs;
using Hr.Application.DTOs.Employee;
using Hr.Application.Interfaces;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.implementation
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IUnitOfWork uniteOfWork;
        private readonly IDepartmentService departmentService;

       

        public EmployeeServices(IUnitOfWork uniteOfWork,  IDepartmentService departmentService)
        {
            this.uniteOfWork = uniteOfWork;
            
            this.departmentService = departmentService;
        }

        #region Employee Attendance
        public IEnumerable<GetAllEmployeAttendanceDto> GetAllEmployeeAttendance()
        {
            var listOfEmployee= new List<GetAllEmployeAttendanceDto>();
            var employes = uniteOfWork.EmployeeRepository.GetAll();
            foreach (var employee in employes)
            {
                var emp = new GetAllEmployeAttendanceDto()
                {
                    ID = employee.Id,
                    Name = employee.FirstName + " " + employee.LastName, 
                };
                listOfEmployee.Add(emp);
            }
            return listOfEmployee;
        }

        public GetAllEmployeAttendanceDto GetAttendanceById(string id)
        {
            var employee = uniteOfWork.EmployeeRepository.Get(x => x.Id == id);
            if(employee == null)
            {
                throw new Exception("Not found Employee");
            }
            else
            {
                var empDto = new GetAllEmployeAttendanceDto()
                {
                    ID = employee.Id,
                    Name = employee.FirstName+" "+ employee.LastName,
                };
                return empDto;
            }
        }

      
        #endregion

        #region Employe

        public bool CheckEmployeeExists(GetAllEmployeeDto EmployeeDto)
        {
          return  uniteOfWork.EmployeeRepository.Any(x => x.FirstName.ToLower() == EmployeeDto.FirstName.ToLower() && x.LastName.ToLower() == EmployeeDto.LastName.ToLower() && x.DepartmentId==EmployeeDto.DepartmentId);
        }

        public IEnumerable<GetAllEmployeeDto> GetAllEmployee()
        {
            var EmployeeList = new List<GetAllEmployeeDto>();
            var employees = uniteOfWork.EmployeeRepository.GetAll(x => x.UserName != SD.AdminUserName, includeProperties: "Department");
            if (employees != null)
            {
                foreach (var emp in employees)
                {
                    var emps = new GetAllEmployeeDto()
                    {
                        ID = emp.Id,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        ArrivalTime = emp.ArrivalTime.ToString("hh\\:mm"), // Convert TimeSpan to string
                        LeaveTime = emp.LeaveTime.ToString("hh\\:mm"), // Convert TimeSpan to string
                        BirthDate = emp.BirthDate,
                        City = emp.City,
                        Country = emp.Country,
                        Gender = emp.Gender,
                        HireDate = emp.HireDate,
                        NationalId = emp.NationalId,
                        Nationality = emp.Nationality,
                        Salary = emp.Salary,
                        DepartmentId = emp.DepartmentId,
                        DeptName = emp.Department.DeptName
                    };
                    EmployeeList.Add(emps);
                }
                return EmployeeList;
            }
            else
            {
                return Enumerable.Empty<GetAllEmployeeDto>();
            }
        }


        public void CreateEmploye(GetAllEmployeeDto EmployeeDto)
        {
            try
            {
                TimeSpan arrivalTime = TimeSpan.Parse(EmployeeDto.ArrivalTime);
                TimeSpan leaveTime = TimeSpan.Parse(EmployeeDto.LeaveTime);
                var empDto = new Employee
                {
                    FirstName = EmployeeDto.FirstName,
                    LastName = EmployeeDto.LastName,
                    ArrivalTime = arrivalTime,
                    LeaveTime = leaveTime,
                    BirthDate = EmployeeDto.BirthDate,
                    City = EmployeeDto.City,
                    Country = EmployeeDto.Country,
                    Gender = EmployeeDto.Gender,
                    HireDate = EmployeeDto.HireDate,
                    NationalId = EmployeeDto.NationalId,
                    Nationality = EmployeeDto.Nationality,
                    Salary = EmployeeDto.Salary,
                    DepartmentId = EmployeeDto.DepartmentId,
                };

                uniteOfWork.EmployeeRepository.Add(empDto);
                uniteOfWork.Save();

            }
            catch (Exception)
            {

                throw new Exception("Employee is error");
            }
            
           
        }

        public GetAllEmployeeDto GetEmployeetId(string id)
        {
            var employees = uniteOfWork.EmployeeRepository.Get(x => x.Id == id, includeProperties: "Department");
            if (employees != null)
            {
                var emps = new GetAllEmployeeDto()
                {
                    ID = employees.Id,
                    FirstName = employees.FirstName,
                    LastName = employees.LastName,
                    ArrivalTime = employees.ArrivalTime.ToString("hh\\:mm"), // Format TimeSpan as "hh:mm"
                    LeaveTime = employees.LeaveTime.ToString("hh\\:mm"), // Format TimeSpan as "hh:mm"
                    BirthDate = employees.BirthDate,
                    City = employees.City,
                    Country = employees.Country,
                    Gender = employees.Gender,
                    HireDate = employees.HireDate,
                    NationalId = employees.NationalId,
                    Nationality = employees.Nationality,
                    Salary = employees.Salary,
                    DepartmentId = employees.DepartmentId,
                    DeptName = employees.Department.DeptName
                };
                return emps;
            }
            else
            {
                return null;
            }
        }

        public void UpdateEmploye(GetAllEmployeeDto EmployeeDto, string id)
        {
            try
            {
                var employeFromDb = uniteOfWork.EmployeeRepository.Get(x => x.Id == id );
                if (employeFromDb != null)
                {
                    TimeSpan arrivalTime = TimeSpan.Parse(EmployeeDto.ArrivalTime);
                    TimeSpan leaveTime = TimeSpan.Parse(EmployeeDto.LeaveTime);

                    employeFromDb.Id = EmployeeDto.ID;
                    employeFromDb.FirstName = EmployeeDto.FirstName;
                    employeFromDb.LastName = EmployeeDto.LastName;
                    employeFromDb.ArrivalTime = arrivalTime; // Use the parsed TimeSpan
                    employeFromDb.LeaveTime = leaveTime; // Use the parsed TimeSpan
                    employeFromDb.BirthDate = EmployeeDto.BirthDate;
                    employeFromDb.City = EmployeeDto.City;
                    employeFromDb.Country = EmployeeDto.Country;
                    employeFromDb.Gender = EmployeeDto.Gender;
                    employeFromDb.HireDate = EmployeeDto.HireDate;
                    employeFromDb.NationalId = EmployeeDto.NationalId;
                    employeFromDb.Nationality = EmployeeDto.Nationality;
                    employeFromDb.Salary = EmployeeDto.Salary;
                    employeFromDb.DepartmentId = EmployeeDto.DepartmentId;
                    uniteOfWork.EmployeeRepository.Update(employeFromDb);
                    uniteOfWork.Save();
                }
                else
                {
                    throw new Exception("Not found");
                }
            }
            catch (Exception)
            {
                throw new Exception("Employee is error");
            }
        }


        public void Remove(string id)
        {
           var emp = uniteOfWork.EmployeeRepository.Get(x=>x.Id==id);
            uniteOfWork.EmployeeRepository.Remove(emp);
            uniteOfWork.Save();
        }

     
        #endregion


    }
}
