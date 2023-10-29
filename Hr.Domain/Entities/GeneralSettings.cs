using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Domain.Entities
{
    public class GeneralSettings
    {
        [Key]
        public int Id { get; set; }
        public int OvertimeHour { get; set; }
        public int DiscountHour { get; set; }

        // Navigation property to Weekends (One GeneralSettings can have many Weekends)
        public ICollection<Weekend> Weekends { get; set; }

        // Navigation property to PublicHolidays (One GeneralSettings can have many PublicHolidays)
        public ICollection<PublicHolidays> PublicHolidays { get; set; }
    }
}
