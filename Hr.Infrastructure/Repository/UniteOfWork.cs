using Hr.Application.Interfaces;
using Hr.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Infrastructure.Repository
{
    public class UniteOfWork: IUniteOfWork
    {
        private readonly ApplicationDbContext context;
        public IAttendanceRepository attendanceRepository {  get; private set; }
        public IEmployeeRepository employeeRepository { get; private set; }
        public UniteOfWork(ApplicationDbContext context)
        {
            this.context = context;
            attendanceRepository = new AttendanceRepository(context);
            employeeRepository = new EmployeeRepository(context);
        }

        public int save()
        {
            return context.SaveChanges();
        }
       
    }
}
