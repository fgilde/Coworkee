using System.Reflection;
using System.Runtime.InteropServices;
using lib.Coworkee.Shared.Constants.Application;

namespace Coworkee.Application.Common.Models
{
    public class VersionInfoModel
    {
        //public string Application => this.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public VersionInfoModel()
        {
            
            AssemblyVersion = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public string ApplicationName { get; set; } = ApplicationConstants.ApplicationName;
        public string AssemblyVersion { get; set; }
        public string Runtime { get; set; } = $"{RuntimeInformation.FrameworkDescription} - {RuntimeInformation.ProcessArchitecture}";
        public string System { get; set; } = $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}";
    }
}