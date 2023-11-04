using Hr.Application.Interfaces;
using Hr.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public IDepartmentRepository DepartmentRepository { get; private set; }

        public IPublicHolidaysRepository PublicHolidaysRepository { get; private set; }
        public IWeekendRepository WeekendRepository { get; private set; }
        public IGeneralSettingsRepository GeneralSettingsRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            DepartmentRepository = new DepartmentRepository(context);
            PublicHolidaysRepository=new PublicHolidaysRepository(context);
            WeekendRepository = new WeekendRepository(context);
            GeneralSettingsRepository =new GeneralSettingsRepository(context);
        }
        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
