using Hr.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetAllDepartment();
        Department GetDepartmentId(int id);
        void Create(Department department);
        void Update(Department department);

        bool CheckDepartmentExists(Department department);
        void Remove(Department department);
    }
}
