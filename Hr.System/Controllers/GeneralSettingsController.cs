using Hr.Application.Common.Enums;
using Hr.Application.DTOs;
using Hr.Application.DTOs.Employee;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.Metrics;




namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralSettingsController : ControllerBase
    {
        IWeekendService weekendService;
        IGeneralSettingsService generalSettingsService;
        IEmployeeServices employeeServices;
        public GeneralSettingsController(IWeekendService weekendService, IGeneralSettingsService generalSettingsService, IEmployeeServices employeeServices)
        {
            this.weekendService = weekendService;
            this.generalSettingsService = generalSettingsService;
            this.employeeServices= employeeServices;
        }
        #region all emps

        [HttpGet]
        //Display CheckBox for Weekends
        public ActionResult Create()
        {
            try
            {
                var general = generalSettingsService.GetGeneralSettingForAll();

                var weekDays = weekendService.Days();
                var weekendDTO = new WeekendDTO();
                if (general == null)
                {
                     weekendDTO = new WeekendDTO
                    {
                        Weekends = weekDays.Select(day => new WeekendCheckDTO { displayValue = day }).ToList()
                    };
                }
                else
                {
                     weekendDTO = new WeekendDTO
                    {
                        OvertimeHour = general.OvertimeHour,
                        DiscountHour = general.DiscountHour,
                        Id = general.Id,
                        empid = general.EmployeeId,
                        Weekends = weekDays.Select(day => new WeekendCheckDTO { displayValue = day }).ToList()
                    };
                }
                IEnumerable<GetAllEmployeeDto> employeeDTOs = employeeServices.GetAllEmployee();
                IEnumerable<SelectListItem> employeeSelectList = employeeDTOs.Select(dto => new SelectListItem
                {
                    Value = dto.ID.ToString(),
                    Text = dto.FirstName
                }).ToList();
                weekendDTO.EmployeeList = employeeSelectList;


                return Ok(weekendDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        #endregion

        #region custom 

        [HttpGet("{empid}")]
        //when relation between emp & his settings (custom Settings)
        public ActionResult GetById(int empid)
        {
            try
            {
                var general = generalSettingsService.GetGeneralSettingId(empid);
                
                if (general==null)
                {
                    var generalNull = generalSettingsService.GetGeneralSettingForAll();

                    var WeekendNull = weekendService.GetById(generalNull.Id);
                    if (WeekendNull.Count() != 0)
                    {
                        var DTONull = new WeekendDTO
                        {
                            Id = generalNull.Id,
                            OvertimeHour = generalNull.OvertimeHour,
                            DiscountHour = generalNull.DiscountHour,
                            empid = generalNull.EmployeeId,
                            Weekends = WeekendNull.Select(day => new WeekendCheckDTO { displayValue = day.Name, isSelected = true }).ToList()
                        };
                        return Ok(DTONull);
                    }
                    else
                    {
                        return NotFound(new
                        {
                            message = "There is no settings for this employee",
                        });
                    }
                
                }
                else
                {
                    var Weekends = weekendService.GetById(general.Id);
                    if (Weekends.Count() != 0)
                    {
                        var DTO = new WeekendDTO
                        {
                            Id = general.Id,
                            OvertimeHour = general.OvertimeHour,
                            DiscountHour = general.DiscountHour,
                            empid= general.EmployeeId,
                            Weekends = Weekends.Select(day => new WeekendCheckDTO { displayValue = day.Name, isSelected = true }).ToList()
                        };
                        return Ok(DTO);
                    }
                    else
                    {
                         var  Deafultweekend= new WeekendDTO();
                        return Ok(Deafultweekend);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        public IActionResult UpdateGeneralSettings(WeekendDTO updatedSettings)
        {

            GeneralSettings updated = null;
            try
            {
                if (updatedSettings.empid !=0)
                {
                    var exitedEmployee = generalSettingsService.CheckEmployeeExists(updatedSettings.empid);
                    if (exitedEmployee == false)
                    {
                        return BadRequest(new { error = "employee Not Found!" });
                    }
                    int nonNullableEmpid = updatedSettings.empid.Value; 
                    updated = generalSettingsService.GetGeneralSettingId(nonNullableEmpid);
                }
                else
                {
                    updated = generalSettingsService.GetGeneralSettingForAll();
                    updatedSettings.empid = null;
                }
               
                if (updated == null)
                {
                    return NotFound();
                }
                //var exitedWeekend =  generalSettingsService.CheckWeekendById(updatedSettings.Id);
                //if(exitedWeekend == false)
                //{
                //    return BadRequest(new { error = "Weekend Not Found!" });
                //}
                    var states = weekendService.Update(updatedSettings,updated.Id);
                if (states == false)
                {
                    return BadRequest( new { error = "Invalid request data." });
                }
                else
                {
                    updated.OvertimeHour = updatedSettings.OvertimeHour;
                    updated.DiscountHour = updatedSettings.DiscountHour;
                    generalSettingsService.Update(updated);
                    return Ok(new { message = "Updated General Settings successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult create(WeekendDTO updatedWeekends)
        {
            try
            {
                int Counter = 0;
                foreach (var item in updatedWeekends.Weekends)
                {
                    if (!item.isSelected)
                    {
                        Counter++;
                    }

                }
                if (Counter == 7)
                {
                    return BadRequest("please select day!");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest( new { updatedWeekends });
                }
                var generalExists = generalSettingsService.CheckGeneralSettingsExists(updatedWeekends.empid);
                if (generalExists)
                {
                    return BadRequest( new {  error= "general settings already exists!" });
                }
                if(updatedWeekends.empid == 0)
                {
                    updatedWeekends.empid = null;
                    var existsNull = generalSettingsService.GetGeneralSettingForAll();
                    if (existsNull != null)
                    {
                        return BadRequest(new { error = "general settings  already exists!" });
                    }
                }
                  var exitedEmployee= generalSettingsService.CheckEmployeeExists(updatedWeekends.empid);
                if (exitedEmployee ==false)
                {
                    return BadRequest(new { error = "employee Not Found!" });
                }
                    var general = new GeneralSettings
                {
                    OvertimeHour = updatedWeekends.OvertimeHour,
                    DiscountHour = updatedWeekends.DiscountHour,
                    EmployeeId=updatedWeekends.empid
                };
                generalSettingsService.Create(general);
                var selectedWeekends = updatedWeekends.Weekends.Where(x => x.isSelected).Select(x => x.displayValue).ToList();
                var created = new List<WeekendDTO>();
                foreach (var selectedDay in selectedWeekends)
                {

                    var weekend = new Weekend
                    {
                        Name = selectedDay,
                        GeneralSettingsId = general.Id
                    };
                    if (weekendService.CheckPublicHolidaysExists(weekend))
                    {
                        return BadRequest($"the day {weekend.Name} allredy selected before!");
                    }
                    weekendService.Create(weekend);
                    created.Add(updatedWeekends);
                }

                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }
        }

        [HttpDelete]
        public ActionResult Remove(int id)
        {
            try
            {
                var remove = generalSettingsService.GetGeneralSettingId(id);
                if (remove == null)
                {
                    return NotFound(new {error="Not Found"});
                }
                var weekendExited= weekendService.GetById(remove.Id);
                if (weekendExited.Count() != 0)
                {
                    foreach (var item in weekendExited)
                    {
                        weekendService.Delete(item);

                    }
                }
                    generalSettingsService.Remove(remove);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred", message = ex.Message });
            }

        }

        #endregion
    }
}





