using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;

namespace Coworkee.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public abstract class BaseDtoMetaConfiguration<T, TId> : IObjectMetaConfiguration<T> where T : IDtoBase<TId>
{
    public virtual Task ConfigureAsync(ObjectEditMeta<T> meta)
    {
        meta.Property(x => x.Id)
            .WithAdditionalAttributes(true,new KeyValuePair<string, object>(nameof(MudBaseInput<TId>.Disabled), true))
            .WithOrder(0);
        if (meta.Value.IsNew)
            meta.Property(x => x.Id).Ignore();
        meta.Property(x => x.IsNew).Ignore();
        meta.WrapEachInMudItem(i =>
        {
            i.xs = 12;
            i.md = 6;
        });
        return Task.CompletedTask;
    }
}