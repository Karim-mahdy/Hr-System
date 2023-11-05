using Hr.Application.DTOs;
using Hr.Application.Interfaces;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.implementation
{
    public class AttendanceServices : IAttendanceServices
    {
        private readonly IUnitOfWork uniteOfWork;
        private readonly IEmployeeServices employeeServices;

        public AttendanceServices(IUnitOfWork uniteOfWork,IEmployeeServices employeeServices)
        {
            this.uniteOfWork = uniteOfWork;
            this.employeeServices = employeeServices;
        }

        

        public AttendanceEmployeDto GetAttendanceById(int id)
        {
           var attendance= uniteOfWork.AttendanceRepository.Get(x=>x.Id == id);
            if(attendance != null)
            {
                var attendanceDto = new AttendanceEmployeDto()
                {
                    Id= attendance.Id,
                    ArrivalTime=attendance.ArrivalTime,
                    LeaveTime=attendance.LeaveTime,
                    Date = attendance.Date,
                    SelectedEmployee=attendance.EmployeeId
                };
                return attendanceDto;
            }
            else
            {
                throw new Exception("Not Found");
            }
        }

      
        public IEnumerable<AttendanceEmployeDto> GetAllAttendance()
        {
            var attendanceDto = new List<AttendanceEmployeDto>();
            var attendances = uniteOfWork.AttendanceRepository.GetAll();
            if(attendances != null )
            {
                foreach (var attendance in attendances)
                {
                    var employee = employeeServices.GetAttendanceById(attendance.EmployeeId);
                    var employeAttendance = new AttendanceEmployeDto()
                    {
                        Id= attendance.Id,
                        SelectedEmployee = employee.Id,
                        Date = attendance.Date,
                        ArrivalTime = attendance.ArrivalTime,
                        LeaveTime = attendance.LeaveTime,
                        
                    };
                    employeAttendance.EmployeeName = employee.Name;
                    attendanceDto.Add(employeAttendance);
                }
                return attendanceDto;
            }
            else
            {
                throw new Exception("No Attendance is Founded");
            }
        }

        public void CreateAttendance(AttendanceEmployeDto attendanceDto)
        {
            var employees = employeeServices.GetAllEmployeeAttendance();
            attendanceDto.EmployeeList = employees.Select(employee => new SelectListItem
            {
                Value = employee.Id.ToString(),
                Text = employee.Name
            });
            var attendance = new Attendance()
            {
                ArrivalTime = attendanceDto.ArrivalTime,
                LeaveTime = attendanceDto.LeaveTime,
                Date = attendanceDto.Date,
                EmployeeId = attendanceDto.SelectedEmployee,
                
            };
            if (attendance != null)
            {
                uniteOfWork.AttendanceRepository.Add(attendance);
                uniteOfWork.Save();
            }
            else
            {
                throw new Exception("Attendance is error");
            }

        }

        public void UpdateAttendance(AttendanceEmployeDto attendanceDto,int id)
        {
            var attendanceFromDb = uniteOfWork.AttendanceRepository.Get(x => x.Id == id);
            if(attendanceFromDb != null )
            {
                var employees = employeeServices.GetAllEmployeeAttendance();
                var employeeList = employees.Select(employee => new SelectListItem
                {
                    Value = employee.Id.ToString(),
                    Text = employee.Name,
                    Selected = (employee.Id == attendanceDto.SelectedEmployee)
                });

                attendanceFromDb.Date = attendanceDto.Date;
                attendanceFromDb.ArrivalTime = attendanceDto.ArrivalTime;
                attendanceFromDb.LeaveTime = attendanceDto.LeaveTime;
                attendanceFromDb.EmployeeId = attendanceDto.SelectedEmployee;
                uniteOfWork.AttendanceRepository.update(attendanceFromDb);
                uniteOfWork.Save();
            }
            else
            {
                throw new Exception("Not found");
            }
        }


        public bool DeleteAttendance(int id)
        {
            var attandence= uniteOfWork.AttendanceRepository.Get( x => x.Id == id);
            if( attandence != null )
            {
                uniteOfWork.AttendanceRepository.Remove(attandence);
                uniteOfWork.Save();
                return true;
            }
            else
            {
                return false;
            }
        }


        public (int BonusHours, int DiscountHours) CalculateBonusAndDiscountHours(int employeeId, int month)
        {
            return uniteOfWork.AttendanceRepository.CalculateBonusAndDiscountHours(employeeId, month);
        }

    }
}
