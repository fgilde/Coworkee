using System;

namespace Coworkee.Shared.Constants.Permission
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