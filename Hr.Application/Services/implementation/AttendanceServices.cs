using Hr.Application.DTO;
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
        private readonly IUniteOfWork uniteOfWork;
        private readonly IEmployeeServices employeeServices;

        public AttendanceServices(IUniteOfWork uniteOfWork,IEmployeeServices employeeServices)
        {
            this.uniteOfWork = uniteOfWork;
            this.employeeServices = employeeServices;
        }

        public void CreateAttendance(AttendanceEmployeDto attendanceDto)
        {
            var employees = employeeServices.GetAllEmployee();
            attendanceDto.EmployeeList = employees.Select(employee => new SelectListItem
            {
                Value = employee.ID.ToString(),
                Text = employee.Name 
            });
            var attendance = new Attendance()
            {
                ArrivalTime = attendanceDto.ArrivalTime,
                LeaveTime = attendanceDto.LeaveTime,
                Date = attendanceDto.Date,
                EmployeeId = attendanceDto.SelectedEmployee,
                Absent = false
            };
            if(attendance != null )
            {
                uniteOfWork.attendanceRepository.Add(attendance);
                uniteOfWork.save();
            }
            else
            {
                throw new Exception("Attendance is error");
            }

        }

        public AttendanceEmployeDto GetAttendanceById(int id)
        {
           var attendance= uniteOfWork.attendanceRepository.Get(x=>x.Id == id);
            if(attendance != null)
            {
                var attendanceDto = new AttendanceEmployeDto()
                {
                    Id= attendance.Id,
                    ArrivalTime=attendance.ArrivalTime,
                    LeaveTime=attendance.LeaveTime,
                    Absent=attendance.Absent,
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
            var attendances = uniteOfWork.attendanceRepository.GetAll();
            if(attendances != null )
            {
                foreach (var attendance in attendances)
                {
                    var employee = employeeServices.GetById(attendance.EmployeeId);
                    var employeAttendance = new AttendanceEmployeDto()
                    {
                        Id= attendance.Id,
                        SelectedEmployee = employee.ID.ToString(),
                        Date = attendance.Date,
                        ArrivalTime = attendance.ArrivalTime,
                        LeaveTime = attendance.LeaveTime,
                        Absent= attendance.Absent
                    };
                    employeAttendance.EmployeeName = employee.Name;
                    attendanceDto.Add(employeAttendance);
                }
                return attendanceDto;
            }
            else
            {
                throw new Exception("Attendance is error");
            }
        }

        public void UpdateAttendance(AttendanceEmployeDto attendanceDto,int id)
        {
            var attadanceFromDb = uniteOfWork.attendanceRepository.Get(x => x.Id == id);
            if(attadanceFromDb != null )
            {
                var employees = employeeServices.GetAllEmployee();
                var employeeList = employees.Select(employee => new SelectListItem
                {
                    Value = employee.ID.ToString(),
                    Text = employee.Name,
                    Selected = (employee.ID == attendanceDto.SelectedEmployee)
                });
                var attendance = new Attendance()
                {
                    Id = attendanceDto.Id,
                    Date = attendanceDto.Date,
                    ArrivalTime = attendanceDto.ArrivalTime,
                    LeaveTime = attendanceDto.LeaveTime,
                    Absent = false,
                    EmployeeId = attendanceDto.SelectedEmployee
                };
                uniteOfWork.attendanceRepository.Update(attendance);
                uniteOfWork.save();
            }
            else
            {
                throw new Exception("Not found");
            }
        }


        public bool DeleteAttendance(int id)
        {
            var attandence= uniteOfWork.attendanceRepository.Get( x => x.Id == id);
            if( attandence != null )
            {
                uniteOfWork.attendanceRepository.Remove(attandence);
                uniteOfWork.save();
                return true;
            }
            else
            {
                return false;
            }
        }


        public (int BonusHours, int DiscountHours) CalculateBonusAndDiscountHours(int employeeId, int month)
        {
            return uniteOfWork.attendanceRepository.CalculateBonusAndDiscountHours(employeeId, month);
        }

    }
}
