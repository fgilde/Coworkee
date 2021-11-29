using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitectureBase.Application.Common.Models;

public class AddUpdateResult<TDto> 
    where TDto : IDtoBase
{
    public AddUpdateResult(IEnumerable<TDto> added = null, IEnumerable<TDto> updated = null, IEnumerable<TDto> skipped = null)
    {
        Added = added?.ToArray() ?? Array.Empty<TDto>();
        Updated = updated?.ToArray() ?? Array.Empty<TDto>();
        Skipped = skipped?.ToArray() ?? Array.Empty<TDto>();
    }
    
    public TDto[] Added { get; set; }
    public TDto[] Updated { get; set; }
    public TDto[] Skipped { get; set; }
}