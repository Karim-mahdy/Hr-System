using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Interfaces
{
    public interface IUnitOfWork
    {

        IAttendanceRepository AttendanceRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        int Save();
    }
}
