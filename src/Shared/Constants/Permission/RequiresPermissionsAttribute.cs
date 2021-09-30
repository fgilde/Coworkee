using System;

namespace CleanArchitectureBase.Shared.Constants.Permission
{
    public class RequiresPermissionsAttribute: Attribute
    {
        public string[] Permissions { get; }

        public RequiresPermissionsAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
    }
}