﻿using Hr.Application.DTOs;
using Hr.Application.DTOs.Employee;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hr.Application.Services.implementation
{
    public class AttendanceServices : IAttendanceServices
    {
        private readonly IUnitOfWork uniteOfWork;
        private readonly IEmployeeServices employeeServices;
        private readonly IGeneralSettingsService generalSettingsService;

        public AttendanceServices(IUnitOfWork uniteOfWork, IEmployeeServices employeeServices, IGeneralSettingsService generalSettingsService)
        {
            this.uniteOfWork = uniteOfWork;
            this.employeeServices = employeeServices;
            this.generalSettingsService = generalSettingsService;
        }



        public AttendanceEmployeDto GetAttendanceById(int id)
        {
            var attendance = uniteOfWork.AttendanceRepository.Get(x => x.Id == id);
            if (attendance != null)
            {
                var attendanceDto = new AttendanceEmployeDto()
                {
                    Id = attendance.Id,
                    ArrivalTime = attendance.ArrivalTime.ToString("hh\\:mm\\:ss"),
                    LeaveTime = attendance.LeaveTime?.ToString("hh\\:mm\\:ss"),
                    Date = attendance.Date,
                    SelectedEmployee = attendance.EmployeeId
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
            if (attendances != null)
            {
                foreach (var attendance in attendances)
                {
                    var employee = employeeServices.GetAttendanceById(attendance.EmployeeId);
                    var employeAttendance = new AttendanceEmployeDto()
                    {
                        Id = attendance.Id,
                        SelectedEmployee = employee.Id,
                        Date = attendance.Date,
                        ArrivalTime = attendance.ArrivalTime.ToString("hh\\:mm\\:ss"),
                        LeaveTime = attendance.LeaveTime?.ToString("hh\\:mm\\:ss"),

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


        public bool CheckAttendanceExists(AttendanceEmployeDto attendanceDto)
        {
            return uniteOfWork.AttendanceRepository.Any(x => x.Date == attendanceDto.Date && x.EmployeeId == attendanceDto.SelectedEmployee);
        }

        public string GetDayOfWeekForDate(DateTime date)
        {
            return date.DayOfWeek.ToString();
        }

        public List<string> GetEmployeeWeekendDays(int employeeId)
        {
            var weekendDays = new List<string>(); // Change the type to string
            var employee = uniteOfWork.EmployeeRepository.Get(x => x.Id == employeeId, includeProperties: "GeneralSettings");
            int generalId = 0;
            var getall = generalSettingsService.GetAllGeneralSettings();
            var generalSettingsWithNullEmployeeId = generalSettingsService.GetGeneralSettingForAll();
       //     var generalSettingsWithNullEmployeeId = uniteOfWork.GeneralSettingsRepository
       //.GetAll()
       //.Where(generalSettings => generalSettings.EmployeeId == null)
       //.FirstOrDefault();
            if (generalSettingsWithNullEmployeeId != null)
            {
                generalId = generalSettingsWithNullEmployeeId.Id;
            }
            if (employee != null)
            {
                var generalSettings = employee.GeneralSettings;

                if (generalSettings.Count != 0)
                {
                    foreach (var setting in generalSettings) // Iterate over the collection
                    {
                        weekendDays.AddRange(generalSettingsService.GetWeekendDaysForGeneralSettings(setting.Id));
                    }
                }
                else
                {
                    var matchingWeekends = uniteOfWork.WeekendRepository.GetAll(weekend => weekend.GeneralSettingsId == generalId);

                    weekendDays.AddRange(matchingWeekends.Select(weekend => weekend.Name));
                }
            }

            return weekendDays;
        }

        public void CreateAttendance(AttendanceEmployeDto attendanceDto)
        {
            try
            {
                TimeSpan arrivalTime = TimeSpan.Parse(attendanceDto.ArrivalTime);
                var attendance = new Attendance()
                {
                    ArrivalTime = arrivalTime,
                    LeaveTime = TimeSpan.Zero,
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateAttendance(AttendanceEmployeDto attendanceDto, int id)
        {
            try
            {
                var attendanceFromDb = uniteOfWork.AttendanceRepository.Get(x => x.Id == id);
                TimeSpan arrivalTime = TimeSpan.Parse(attendanceDto.ArrivalTime);
                TimeSpan leaveTime = TimeSpan.Parse(attendanceDto.LeaveTime);
                if (attendanceFromDb != null)
                {
                    attendanceFromDb.Date = attendanceDto.Date;
                    attendanceFromDb.ArrivalTime = arrivalTime;
                    attendanceFromDb.LeaveTime = leaveTime;
                    attendanceFromDb.EmployeeId = attendanceDto.SelectedEmployee;
                    uniteOfWork.AttendanceRepository.update(attendanceFromDb);
                    uniteOfWork.Save();
                }
                else
                {
                    throw new Exception("Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool DeleteAttendance(int id)
        {
            var attandence = uniteOfWork.AttendanceRepository.Get(x => x.Id == id);
            if (attandence != null)
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
    }
}
