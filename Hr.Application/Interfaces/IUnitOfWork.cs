using Hr.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IDepartmentRepository DepartmentRepository { get; }
        IPublicHolidaysRepository PublicHolidaysRepository { get; }
        IWeekendRepository WeekendRepository { get; }
        IGeneralSettingsRepository GeneralSettingsRepository { get; }
        int Save();
    }
}
