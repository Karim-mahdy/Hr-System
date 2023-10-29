using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Domain.Entities
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan LeaveTime { get; set; }
        public bool Absent { get; set; }
        public double? BonusHour { get; set; }
        public double? DiscountHour { get; set; }

        // Navigation property to Employee
        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
