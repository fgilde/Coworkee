using System.ComponentModel;

namespace Coworkee.Application.Contracts.Enums;

public enum ExportServiceType
{
    /// <summary>
    /// Export as Excel
    /// </summary>
    [Description(nameof(Excel))] 
    Excel,

    /// <summary>
    /// Export as CSV
    /// </summary>
    [Description(nameof(Csv))]
    Csv,

    /// <summary>
    /// Export as Json
    /// </summary>
    [Description(nameof(Json))] 
    Json
}