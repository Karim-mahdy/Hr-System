using Hr.Application.Common;
using Hr.Application.DTOs.User;
using Hr.Application.Services.implementation;
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
        private readonly UserManager<Employee> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRoleService roleService;
        private readonly ApplicationDbContext context;

        public UserController(UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager,
            IRoleService roleService,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.roleService = roleService;
            this.context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await userManager.Users.Where(x => x.UserName != "Admin@gmail.com").ToListAsync();
                if (users != null)
                {
                    var userRoleDto = new List<UserWithRoleDto>();
                    foreach (var user in users)
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        var userDto = new UserWithRoleDto
                        {

                            User = $"{user.FirstName} {user.LastName}",
                            Roles = roles.ToList()
                        };
                        userRoleDto.Add(userDto);
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
                return NotFound(); // Return an error response if the user is not found.
            }

            var roles = await userManager.GetRolesAsync(user);
            var userRoles = roleManager.Roles
                            .Where(r => roles.Contains(r.Name))
                            .Select(r => new IdentityRole { Id = r.Id, Name = r.Name })
                            .ToList();

            //get all roles for user
            var roleIds = roles.Select(roleName => roleManager.Roles.SingleOrDefault(r => r.Name == roleName)?.Id).ToList();
            var UserDto = new UserRoleFromDto
            {
                selectEmployeeId = user.Id,
                EmployeeName = user.FirstName + " " + user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                userRoles = userRoles,
                selectRolesIds= roleIds,
                Roles = GetRolesList(),
            };

            return Ok(UserDto);
        }

        [HttpGet("GetToCreate")]
        public async Task<IActionResult> GetToCreate()
        {
            try
            {
                var UserDto = new UserRoleFromDto
                {
                    UserName = "",
                    Email = "",
                    Password = "",
                    Roles = GetRolesList(),
                    Employees = GetUserSelectList()

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
                    var user = await userManager.FindByIdAsync(model.selectEmployeeId);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                   

                    var result = await userManager.CreateAsync(user, model.Password);

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

        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser(UserRoleFromDto model)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByIdAsync(model.selectEmployeeId);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.UserName = model.UserName;
                    user.Email = model.Email;

                    // To update the user's password, use the ChangePasswordAsync method.
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await userManager.ResetPasswordAsync(user, token, model.Password);

                        if (!result.Succeeded)
                        {
                            transaction.Rollback();
                            return BadRequest("Failed to update the user's password");
                        }
                    }

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

                    // If any step fails, roll back the transaction.
                    transaction.Rollback();
                }

                return BadRequest("Failed to edit user or update user roles");
            }
        }

        [HttpPost("RemoveRolesFromUser")]
        public async Task<IActionResult> RemoveRolesFromUser(string userId, List<string> roleNames)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await userManager.RemoveFromRolesAsync(user, roleNames);

            if (result.Succeeded)
            {
                return Ok("Roles removed successfully");
            }

            return BadRequest("Failed to remove roles from the user");
        }


        private IEnumerable<SelectListItem> GetRolesList()
        {
            return roleManager.Roles.Select(role => new SelectListItem
            {
                Value = role.Id,
                Text = role.Name
            });
        }
        private IEnumerable<SelectListItem> GetUserSelectList()
        {
            return userManager.Users.Where(x => x.UserName != "Admin@gmail.com").Select(x => new SelectListItem
            {
                Text = $"{x.FirstName} {x.LastName}",
                Value = x.Id
            }).ToList();
        }



    }

}
