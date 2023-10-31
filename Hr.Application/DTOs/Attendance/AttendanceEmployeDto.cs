using Hr.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.DTOs
{
    public class AttendanceEmployeDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan LeaveTime { get; set; }
        public bool? Absent { get; set; }=true;
        public string? SelectedEmployee { get; set; }
        public string? EmployeeName { get; set; }

        public IEnumerable<SelectListItem>? EmployeeList { get; set; } = Enumerable.Empty<SelectListItem>();

    }
}
