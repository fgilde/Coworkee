using System.Reflection;
using System.Runtime.InteropServices;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.Extensions.PlatformAbstractions;

namespace CleanArchitectureBase.Application.Common.Models
{
    public class VersionInfoModel
    {
        //public string Application => this.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public VersionInfoModel()
        {
            AssemblyVersion = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public string ApplicationName { get; set; } = ApplicationConstants.ApplicationName;
        public string ApplicationVersion { get; set; } = ApplicationConstants.Version;
        public string AssemblyVersion { get; set; }
        public string Runtime { get; set; } = PlatformServices.Default.Application.RuntimeFramework.FullName;
        public string System { get; set; } = $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}";
    }
}
