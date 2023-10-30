using Hr.Application.Interfaces;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool CheckDepartmentExists(Department department)
        {
            return unitOfWork.DepartmentRepository.Any(x => x.DeptName.ToLower() == department.DeptName.ToLower());
        }

        public void Create(Department department)
        {
            unitOfWork.DepartmentRepository.Add(department);
            unitOfWork.Save();
        }

        public IEnumerable<Department> GetAllDepartment()
        {
            return unitOfWork.DepartmentRepository.GetAll();
        }

        public Department GetDepartmentId(int id)
        {
            return unitOfWork.DepartmentRepository.Get(x => x.Id == id);
        }

        public void Update(Department department)
        {
            unitOfWork.DepartmentRepository.Update(department);
            unitOfWork.Save();
        }

        public void Remove(Department department)
        {
            unitOfWork.DepartmentRepository.Remove(department);
            unitOfWork.Save();
        }
    }

}
