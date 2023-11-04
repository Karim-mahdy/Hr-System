using Hr.Application.DTOs;
using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.Interfaces
{
    public interface IGeneralSettingsService
    {
        IEnumerable<GeneralSettings> GetAllGeneralSettings();
        GeneralSettings GetGeneralSettingId(int id);
        void Create(GeneralSettings generalSettings);
        void Update(GeneralSettings generalSettings);
        //bool CheckPublicHolidaysExists(PublicHolidaysDTO publicHolidayDTO);
        void Remove(GeneralSettings generalSettings);
    }
}
