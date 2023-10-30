using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Interfaces
{
    public interface IUniteOfWork
    {
        IAttendanceRepository attendanceRepository { get; }
        IEmployeeRepository employeeRepository { get; }
        int save();
    }
}
