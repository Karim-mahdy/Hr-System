using Hr.Application.DTO;
using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.Interfaces
{
    public interface IAttendanceServices
    {
        IEnumerable<AttendanceEmployeDto> GetAllAttendance();
        AttendanceEmployeDto GetAttendanceById(int id);
        void CreateAttendance(AttendanceEmployeDto attendanceDto);
        void UpdateAttendance(AttendanceEmployeDto attendanceDto, int id);
        bool DeleteAttendance(int id);

        public (int BonusHours, int DiscountHours) CalculateBonusAndDiscountHours(int employeeId, int month);
    }
}
