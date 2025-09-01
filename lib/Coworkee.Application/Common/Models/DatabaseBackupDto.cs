using Nextended.Core.Helper;
using System;

namespace Coworkee.Application.Common.Models;

public class DatabaseBackupDto : IDtoBase
{
    public string Name { get; set; }
    public long Size { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsNew => false;
    public string FileSize => FileHelper.GetReadableFileSize(Size);
}