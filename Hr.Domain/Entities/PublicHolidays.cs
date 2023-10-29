using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Domain.Entities
{
    public class PublicHolidays
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Day { get; set; }

        [ForeignKey("GeneralSettings")]
        public int GeneralSettingsId { get; set; }

        public GeneralSettings GeneralSettings { get; set; }
    }
}
