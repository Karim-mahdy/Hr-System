using Hr.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.DTOs
{
    public class AttendanceEmployeDto
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Arrival Time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan ArrivalTime { get; set; }
        [Required(ErrorMessage = "Leave Time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan LeaveTime { get; set; }
        public bool? Absent { get; set; }=true;
        public int SelectedEmployee { get; set; }
        public string? EmployeeName { get; set; }

        public IEnumerable<SelectListItem>? EmployeeList { get; set; } = Enumerable.Empty<SelectListItem>();

    }
}
