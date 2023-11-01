using AutoMapper;
using Hr.Application.DTOs.Employee;
using Hr.Domain.Entities;

namespace Hr.System.Mapping
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee, GetAllEmployeeDto>();
            // Add more mappings as needed
        }
    }
}
