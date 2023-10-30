using Hr.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Application.Common.Global
{
    public static class Permission
    {
        public static List<string> GeneratePermissionList(string module)
        {
            return new List<string>()
            {
                $"Permission.{module}.Create",
                $"Permission.{module}.View",
                $"Permission.{module}.Edit",
                $"Permission.{module}.Delete"
            };
        }
        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();
            var modules = Enum.GetValues(typeof(Modules));
            foreach (var module in modules)
            {
                allPermissions.AddRange(GeneratePermissionList(module.ToString()));
            }
            return allPermissions;
        }
    }
}
