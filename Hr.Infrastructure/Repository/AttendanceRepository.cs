using Hr.Application.Interfaces;
using Hr.Domain.Entities;
using Hr.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Infrastructure.Repository
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        private readonly ApplicationDbContext context;

        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void update(Attendance attendance)
        {
            context.Attendances.Update(attendance);
        }



        public (int BonusHours, int DiscountHours) CalculateBonusAndDiscountHours(int employeeId, int month)
        {
            var BonusHours = new SqlParameter("@BonusHours", SqlDbType.Int);
            BonusHours.Direction = ParameterDirection.Output;

            var discountHours = new SqlParameter("@DiscountHours", SqlDbType.Int);
            discountHours.Direction = ParameterDirection.Output;

            var result = context.Database.ExecuteSqlRaw(
     "EXEC CalculateBonusAndDiscountHours @EmployeeID, @Month, @BonusHours OUTPUT, @DiscountHours OUTPUT",
     new SqlParameter("@EmployeeID", employeeId),
     new SqlParameter("@Month", month),
     BonusHours,
     discountHours
 );

            int bonusHoursValue = (int)BonusHours.Value;
            int discountHoursValue = (int)discountHours.Value;

            return (bonusHoursValue, discountHoursValue);
        }
    }
}
