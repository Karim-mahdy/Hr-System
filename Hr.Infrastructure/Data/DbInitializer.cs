using Hr.Application.Common;
using Hr.Application.Common.Enums;
using Hr.Application.Common.Global;
using Hr.Application.Interfaces;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Hr.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async void Configure(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Employee>>();

                await roleManager.SeedAdminRoleAsync();
                await userManager.SeedAdminUserAsync(roleManager);
            }

            // Other configuration code
        }
        public static async Task SeedAdminRoleAsync(this RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(SD.Roles.SuperAdmin.ToString()));
        }

        public static async Task SeedAdminUserAsync(this UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminUser = new Employee
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(adminUser, "Admin@123");
               
                await userManager.AddToRoleAsync(adminUser, SD.Roles.SuperAdmin.ToString());
            }
            await roleManager.SeedClaimsToAdmin(adminUser);
        }

        public static async Task SeedClaimsToAdmin(this RoleManager<IdentityRole> roleManager, Employee adminUser)
        {
            var adminRole = await roleManager.FindByNameAsync(SD.Roles.SuperAdmin.ToString());
            if (adminRole != null)
            {
                foreach (Modules module in Enum.GetValues(typeof(Modules)))
                {
                    var allPermissions = Permission.GeneratePermissionList(module.ToString());
                    var allClaims = await roleManager.GetClaimsAsync(adminRole);
                    foreach (var permission in allPermissions)
                    {
                        if (!allClaims.Any(c => c.Type == SD.PermissionType && c.Value == permission))
                            await roleManager.AddClaimAsync(adminRole, new Claim(SD.PermissionType, permission));
                    }
                }
            }
        }

    }


    //public static class DataInitilizer
    //{

    //    public static async void Configure(IApplicationBuilder app)
    //    {


    //        // Seed the admin user and role
    //        using (var serviceScope = app.ApplicationServices.CreateScope())
    //        {
    //            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Employee>>();

    //            await roleManager.SeedAdminRoleAsync();
    //            await userManager.SeedAdminUserAsync(roleManager);
    //        }

    //        // Other configuration code
    //    }

    //    public static async Task SeedAdminRoleAsync(this RoleManager<IdentityRole> roleManager)
    //    {
    //        await roleManager.CreateAsync(new IdentityRole(SD.Roles.SuperAdmin.ToString()));
    //    }

    //    public static async Task SeedAdminUserAsync(this UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
    //    {
    //        var adminUser = new Employee
    //        {
    //            UserName = "Admin@gmail.com",
    //            Email = "Admin@gmail.com",
    //            EmailConfirmed = true
    //        };

    //        var user = await userManager.FindByEmailAsync(adminUser.Email);
    //        if (user == null)
    //        {
    //            await userManager.CreateAsync(adminUser, "Admin@123");
    //            await userManager.AddToRoleAsync(adminUser, SD.Roles.SuperAdmin.ToString());
    //        }
    //        await roleManager.SeedClaimsToAdmin();
    //    }

    //    private static async Task SeedClaimsToAdmin(this RoleManager<IdentityRole> roleManager)
    //    {
    //        var adminRole = await roleManager.FindByNameAsync(SD.Roles.SuperAdmin.ToString());

    //        // Adding Claims (Permissions) To the Admin

    //        await roleManager.AddPermissionClaims(adminRole, "Employee");
    //    }

    //    private static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
    //    {
    //        var allClaims = await roleManager.GetClaimsAsync(role);
    //        var allPermissions = Permission.GeneratePermissionList(module);

    //        foreach (var permission in allPermissions)
    //        {
    //            if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
    //                await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
    //        }
    //    }
    //}

    #region custom
    //public class DbInitializer : IDbInitializer
    //{

    //    private readonly UserManager<Employee> _userManager;
    //    private readonly RoleManager<IdentityRole> _roleManager;
    //    private readonly ApplicationDbContext _db;

    //    public DbInitializer(
    //        UserManager<Employee> userManager,
    //        RoleManager<IdentityRole> roleManager,
    //        ApplicationDbContext db)
    //    {
    //        _roleManager = roleManager;
    //        _userManager = userManager;
    //        _db = db;
    //    }

    //    public async void Initialize()
    //    {
    //        await SeedAdminRoleAsync();
    //        await SeedAdminUserAsync();
    //        await SeedClaimsToAdmin(SD.Roles.SuperAdmin.ToString());
    //    }
    //    public static void Configure(IApplicationBuilder app)
    //    {
    //        using (var serviceScope = app.ApplicationServices.CreateScope())
    //        {
    //            var dbInitializer = serviceScope.ServiceProvider.GetRequiredService<IDbInitializer>();
    //            dbInitializer.Initialize();
    //        }

    //        // Other configuration code
    //    }

    //    private async Task SeedAdminRoleAsync()
    //    {
    //        await _roleManager.CreateAsync(new IdentityRole(SD.Roles.SuperAdmin.ToString()));
    //    }

    //    private async Task SeedAdminUserAsync()
    //    {
    //        var adminUser = new Employee
    //        {
    //            UserName = "Admin@gmail.com",
    //            Email = "Admin@gmail.com",
    //            EmailConfirmed = true
    //        };

    //        var user = await _userManager.FindByEmailAsync(adminUser.Email);
    //        if (user == null)
    //        {
    //            await _userManager.CreateAsync(adminUser, "Admin@123");
    //            // Create and seed the default department
    //            var department = new Department
    //            {
    //                DeptName = "Default Department"
    //                // Add other department properties as needed
    //            };

    //            // Add the department to your DbContext (assuming you're using Entity Framework)
    //            await _db.Departments.AddAsync(department);
    //            await _db.SaveChangesAsync();

    //            adminUser.DepartmentId = department.Id;
    //            await _userManager.UpdateAsync(adminUser);
    //            await _userManager.AddToRoleAsync(adminUser, SD.Roles.SuperAdmin.ToString());
    //        }
    //    }


    //    private async Task SeedClaimsToAdmin(string roleName)
    //    {
    //        var adminRole = await _roleManager.FindByNameAsync(roleName);
    //        if (adminRole != null)
    //        {
    //            foreach (Modules module in Enum.GetValues(typeof(Modules)))
    //            {
    //                var allPermissions = Permission.GeneratePermissionList(module.ToString());
    //                var allClaims = await _roleManager.GetClaimsAsync(adminRole);
    //                foreach (var permission in allPermissions)
    //                {
    //                    if (!allClaims.Any(c => c.Type == SD.PermissionType && c.Value == permission))
    //                        await _roleManager.AddClaimAsync(adminRole, new Claim(SD.PermissionType, permission));
    //                }
    //            }
    //        }
    //    }

    //}

    #endregion
}
