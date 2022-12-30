using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Coworkee.Server
{
    /// <summary>
    /// NOTICE*
    /// To create a new Version for example v3 you need to do 4 steps
    ///  - Add  public const string V3 = "3"; in this class.
    ///  - Change the config "documentName" in file nswag.json to v3 ("documentName": "v2")
    ///  - Add this command to NSwag build target in WebAPI.csproj  Exec Command="$(NSwagExe_Net50) run /v3/swagger.json /variables:Configuration=$(Configuration)" />
    ///  - In Angular Client app open app.module.ts and change API_BASE_URL to 'api/v3'
    /// </summary>
    public static class ApiVersions
    {
        public const string DocumentVersionPrefix = "v";
        public const string GroupNameFormat = "'v'VVV";

        public const string V1 = "1";
        //public const string V2 = "2";

        public static ApiVersion Newest => All.OrderBy(v => v).Last();

        public static IEnumerable<ApiVersion> All
        {
            get
            {
                return typeof(ApiVersions).GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(info => info.Name.StartsWith("V"))
                    .Select(info => info.GetValue(null)?.ToString().AsApiVersion())
                    .Where(v => v != null);
            }
        }

        public static string VersionString(ApiVersion v)
        {
            return v.ToString();
        }

        public static Version ToVersion(this ApiVersion v)
        {
            return new Version(VersionString(v));
        }

        private static ApiVersion AsApiVersion(this string versionStr)
        {
            return ApiVersion.TryParse(versionStr, out var res) && res != null
                ? new ApiVersion(res.MajorVersion ?? 0, res.MinorVersion ?? 0)
                : null;
        }
    }

}