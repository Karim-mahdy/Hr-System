﻿using Hr.Application.Interfaces;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.implementation
{
    public class GeneralSettingsService :IGeneralSettingsService
    {
        IUnitOfWork unitOfWork;
        public GeneralSettingsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        public IEnumerable<GeneralSettings> GetAllGeneralSettings()
        {
            return unitOfWork.GeneralSettingsRepository.GetAll();
        }
        public GeneralSettings GetGeneralSettingId(int id)
        {
           return unitOfWork.GeneralSettingsRepository.Get(x=>x.Id==id);
        }
        public void Create(GeneralSettings generalSettings)
        {
            unitOfWork.GeneralSettingsRepository.Add(generalSettings);
            unitOfWork.Save();
        }
        public void Update(GeneralSettings generalSettings)
        {
            unitOfWork.GeneralSettingsRepository.Update(generalSettings);
            unitOfWork.Save();
        }
        //bool CheckPublicHolidaysExists(PublicHolidaysDTO publicHolidayDTO);
        public void Remove(GeneralSettings generalSettings)
        {
            unitOfWork.GeneralSettingsRepository.Remove(generalSettings);
            unitOfWork.Save();
        }
    }
}
