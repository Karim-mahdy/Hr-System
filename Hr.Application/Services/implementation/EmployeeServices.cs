using AutoMapper;
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

        public IMapper _mapper { get; }

        public EmployeeServices(IUnitOfWork uniteOfWork, IMapper mapper,IDepartmentService departmentService)
        {
            this.uniteOfWork = uniteOfWork;
            _mapper = mapper;
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
          return  uniteOfWork.EmployeeRepository.Any(x => x.FirstName.ToLower() == EmployeeDto.FirstName.ToLower() && x.LastName.ToLower() == EmployeeDto.LastName.ToLower()&& x.DepartmentId==EmployeeDto.DepartmentId);
        }

        public void CreateEmploye(GetAllEmployeeDto EmployeeDto)
        {
            var department = departmentService.GetAllDepartment();
            EmployeeDto.DepartmentList= department.Select(dept=> new SelectListItem
            {
                Value = dept.Id.ToString(),
                Text = dept.Name
            });
            var empDto = new Employee
            {
                FirstName = EmployeeDto.FirstName,
                LastName = EmployeeDto.LastName,
                ArrivalTime = EmployeeDto.ArrivalTime,
                LeaveTime = EmployeeDto.LeaveTime,
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
            if (empDto != null)
            {
                uniteOfWork.EmployeeRepository.Add(empDto);
                uniteOfWork.Save();
            }
            else
            {
                throw new Exception("Employee is error");
            }
        }

        public IEnumerable<GetAllEmployeeDto> GetAllEmployee()
        {
           var EmployeeList= new List<GetAllEmployeeDto>();
            var employees = uniteOfWork.EmployeeRepository.GetAll(includeProperties: "Department");
            if (employees != null)
            {
                foreach (var emp in employees)
                {
                    var emps = new GetAllEmployeeDto()
                    {
                        ID = emp.Id,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        ArrivalTime = emp.ArrivalTime,
                        LeaveTime = emp.LeaveTime,
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
        public GetAllEmployeeDto GetEmployeetId(string id)
        {
            var employees = uniteOfWork.EmployeeRepository.Get(x=>x.Id==id, includeProperties: "Department");
            if (employees != null)
            {
                    var emps = new GetAllEmployeeDto()
                    {
                        ID = employees.Id,
                        FirstName = employees.FirstName,
                        LastName = employees.LastName,
                        ArrivalTime = employees.ArrivalTime,
                        LeaveTime = employees.LeaveTime,
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
            var employeFromDb = uniteOfWork.EmployeeRepository.Get(x => x.Id == id, includeProperties: "Department");
            if (employeFromDb != null)
            {
                var department = departmentService.GetAllDepartment();
                var employeeList = department.Select(dept => new SelectListItem
                {
                    Value = dept.Id.ToString(),
                    Text = dept.Name,
                    Selected = (dept.Id == EmployeeDto.DepartmentId)
                });
                employeFromDb.Id = EmployeeDto.ID;
                employeFromDb.FirstName = EmployeeDto.FirstName;
                employeFromDb.LastName = EmployeeDto.LastName;
                employeFromDb.ArrivalTime = EmployeeDto.ArrivalTime;
                employeFromDb.LeaveTime = EmployeeDto.LeaveTime;
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

        public void Remove(string id)
        {
           var emp = uniteOfWork.EmployeeRepository.Get(x=>x.Id==id);
            uniteOfWork.EmployeeRepository.Remove(emp);
            uniteOfWork.Save();
        }

     
        #endregion


    }
}
