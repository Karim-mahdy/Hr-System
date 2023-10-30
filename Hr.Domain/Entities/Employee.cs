﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Domain.Entities
{
    public class Employee : IdentityUser
    {
       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        public string NationalId { get; set; }
        public DateTime HireDate { get; set; }
        public double Salary { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan LeaveTime { get; set; }

        // Navigation property to Department
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Attendance> Attendance { get; set; }

    }
}
