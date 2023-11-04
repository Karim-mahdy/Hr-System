using Hr.Application.Common;
using Hr.Application.DTOs.User;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Hr.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Hr.Application.Common.SD;

namespace Hr.System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRoleService roleService;
        private readonly ApplicationDbContext context;
        private readonly IEmployeeServices employeeService;

        public UserController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IRoleService roleService,
            ApplicationDbContext context ,
            IEmployeeServices employeeService
            )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.roleService = roleService;
            this.context = context;
            this.employeeService = employeeService;
        }


        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await userManager.Users.Where(x => x.UserName != SD.AdminUserName).ToListAsync();

                if (users != null)
                {
                    var userRoleDto = new List<UserWithRoleDto>();

                    foreach (var user in users)
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        var employee = employeeService.GetEmployeeByUserId(user.Id);

                        if (employee != null)
                        {
                            var userDto = new UserWithRoleDto
                            {
                                FullName = $"{employee.FirstName} {employee.LastName}",
                                UserName = user.UserName,
                                Email = user.Email,
                                Password = user.PasswordHash,
                                Roles = roles.ToList()
                            };
                            userRoleDto.Add(userDto);
                        }
                    }

                    return Ok(userRoleDto);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                // Handle the exception
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string userId)
        {
           
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); 
            }
            // Retrieve the roles associated with the user
            var roles = await userManager.GetRolesAsync(user);

            var employee = employeeService.GetEmployeeByUserId(userId);

            if (employee != null)
            {
                // Map user's roles to IdentityRole objects
                var userRoles = roleManager.Roles
                    .Where(r => roles.Contains(r.Name))
                    .Select(r => new IdentityRole { Id = r.Id, Name = r.Name })
                    .ToList();

                // Get all role IDs for the user
                var roleIds = roles.Select(roleName => roleManager.Roles.SingleOrDefault(r => r.Name == roleName)?.Id).ToList();

                // Create a DTO  with user and role information
                var UserDto = new UserRoleFromDto
                {
                    EmpId = employee.ID,
                    UserId = user.Id,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = user.PasswordHash,
                    userRoles = userRoles,
                    selectRolesIds = roleIds,
                    Roles = GetRolesListExceptUserRoles(user.Id),
                };
                return Ok(UserDto);
            }
            else
            {         
                return NotFound("Employee not found for the user.");
            }
        }


        [HttpGet("GetToCreate")]
        public async Task<IActionResult> GetToCreate()
        {
            try
            {
                var roles =  GetRolesList();
                var employees = await GetUserSelectListWithoutRoles();
                var UserDto = new UserRoleFromDto
                {

                    EmpId = 0,
                    UserName = "",
                    Email = "",
                    Password = "",
                    Roles = roles.ToList(),
                    Employees = employees.ToList()
                };
                return Ok(UserDto);
            }
            catch (Exception ex)
            {
                // Handle the exception
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRoleFromDto model)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    user.PasswordHash = model.Password;
                    
                    var result = await userManager.CreateAsync(user);
                    

                    if (result.Succeeded)
                    {
                        var emp = employeeService.GetEmployeeId(model.EmpId);
                        emp.UserId = user.Id;
                        employeeService.UpdateEmployee(emp,emp.ID);


                        var selectedRoles = await roleManager.Roles
                            .Where(role => model.selectRolesIds.Contains(role.Id))
                            .ToListAsync();

                        var addRolesResult = await userManager.AddToRolesAsync(user, selectedRoles.Select(role => role.Name));

                        if (addRolesResult.Succeeded)
                        {
                            transaction.Commit();
                            return Ok("User created successfully");
                        }
                    }

                    // If any step fails, roll back the transaction.
                    transaction.Rollback();
                }

                return BadRequest("Failed to create the user or assign user roles");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserRoleFromDto model)
        {
             
                using (var transaction = context.Database.BeginTransaction())
                {
                    if (ModelState.IsValid)
                    {
                        var user = await userManager.FindByIdAsync(model.UserId);
                        if (user == null)
                        {
                            return NotFound();
                        }

                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        user.PasswordHash = model.Password;
                        var result = await userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {


                            var selectedRoles = await roleManager.Roles
                                .Where(role => model.selectRolesIds.Contains(role.Id))
                                .ToListAsync();

                            // Remove existing roles
                            var userRoles = await userManager.GetRolesAsync(user);
                            var removeRolesResult = await userManager.RemoveFromRolesAsync(user, userRoles);

                            if (removeRolesResult.Succeeded)
                            {
                                var addRolesResult = await userManager.AddToRolesAsync(user, selectedRoles.Select(role => role.Name));

                                if (addRolesResult.Succeeded)
                                {
                                    transaction.Commit();
                                    return Ok("User roles updated successfully");
                                }
                            }
                        }

                        // If any step fails, roll back the transaction.
                        transaction.Rollback();
                    }

                    return BadRequest("Failed to create user or update user roles");
                }
 
        }


        [HttpPost("RemoveUser")]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            try
            {
                // Remove the user from ASP.NET Identity
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                      
                    var emp = employeeService.GetEmployeeByUserId(userId);
                    emp.UserId =null;
                    employeeService.UpdateEmployee(emp, emp.ID);
                   
                    return Ok("User removed and associated Employee updated.");
                }

                return BadRequest("Failed to remove the user.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, ex.Message);
            }
        }


        private IEnumerable<SelectListItem> GetRolesList()
        {
            return roleManager.Roles.Select(role => new SelectListItem
            {
                Value = role.Id,
                Text = role.Name
            });
        }
        
        private IEnumerable<SelectListItem> GetRolesListExceptUserRoles(string currentUserId)
        {
            var currentUserRoles = userManager.GetRolesAsync(userManager.FindByIdAsync(currentUserId).Result).Result;

            var rolesWithoutCurrentUserRoles = roleManager.Roles
                .Where(role => !currentUserRoles.Contains(role.Name))
                .Select(role => new SelectListItem
                {
                    Value = role.Id,
                    Text = role.Name
                });

            return rolesWithoutCurrentUserRoles;
        }

        //private async Task<IEnumerable<SelectListItem>> GetUserSelectListWithoutRoles()
        //{
        //    // Retrieve users with roles
        //    var usersWithRoles = (await userManager.Users.ToListAsync())
        //        .Where(user => userManager.GetRolesAsync(user).Result.Any())
        //        .ToList();

        //    // Retrieve all users
        //    var allUsers = await userManager.Users
        //        .Where(x => x.UserName != SD.AdminUserName)
        //        .ToListAsync();

        //    // Filter users without roles and select their names
        //    var usersWithoutRoles = allUsers
        //        .Where(x => !usersWithRoles.Any(u => u.Id == x.Id))
        //        .Select(x => $"{x.FirstName} {x.LastName}")
        //        .ToList();

        //    var selectListItems = usersWithoutRoles
        //        .Select(userName => new SelectListItem
        //        {
        //            Text = userName,
        //            Value = userId // You'll need to replace 'userId' with the actual ID.
        //        })
        //        .ToList();

        //    return usersWithoutRoles;
        //}


        private async Task<IEnumerable<SelectListItem>> GetUserSelectListWithoutRoles()
        {
            // Retrieve users with roles
            var usersWithRoles = (await userManager.Users.Where(user =>
                userManager.GetRolesAsync(user).Result.Any()).ToListAsync());

            // Retrieve users without roles
            var usersWithoutRoles = (await userManager.Users
                .Where(user => user.UserName != SD.AdminUserName && !usersWithRoles.Any(u => u.Id == user.Id))
                .ToListAsync());

            // Create SelectListItem instances with user names
            var selectListItems = usersWithoutRoles
                .Select(user => new SelectListItem
                {
                    Text = $"{employeeService.GetEmployeeByUserId(user.Id).FirstName} {employeeService.GetEmployeeByUserId(user.Id).LastName}",
                    Value = user.Id
                })
                .ToList();

            return selectListItems;
        }


    }

}
