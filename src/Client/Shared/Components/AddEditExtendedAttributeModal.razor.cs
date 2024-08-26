using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Coworkee.Application.Features.ExtendedAttributes.Commands.AddEdit;
using Coworkee.Application.Hubs;
using Coworkee.Client.Extensions;
using Coworkee.Client.Managers.ExtendedAttribute;
using Coworkee.Domain.Contracts;
using Coworkee.Domain.Enums;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace Coworkee.Client.Shared.Components
{
    public class AddEditExtendedAttributeModalLocalization
    {
        // for localization
    }

    public partial class AddEditExtendedAttributeModal<TId, TEntityId, TEntity, TExtendedAttribute> : IAsyncDisposable
        where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        [Inject] private IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute> ExtendedAttributeManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> AddEditExtendedAttributeModel { get; set; } = new();

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private MudDatePicker _datePicker;
        private MudTimePicker _timePicker;
        private TimeSpan? _time;
        private DateTime? _date;

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            switch (AddEditExtendedAttributeModel.Type)
            {
                case EntityExtendedAttributeType.Decimal:
                    AddEditExtendedAttributeModel.DateTime = null;
                    AddEditExtendedAttributeModel.Text = null;
                    AddEditExtendedAttributeModel.Json = null;
                    break;
                case EntityExtendedAttributeType.Text:
                    AddEditExtendedAttributeModel.Decimal = null;
                    AddEditExtendedAttributeModel.DateTime = null;
                    AddEditExtendedAttributeModel.Json = null;
                    break;
                case EntityExtendedAttributeType.DateTime:
                    var d = _date ?? DateTime.Now;
                    AddEditExtendedAttributeModel.DateTime = DateOnly.FromDateTime(d).ToDateTime(TimeOnly.FromTimeSpan(_time ?? d.TimeOfDay));
                    AddEditExtendedAttributeModel.Decimal = null;
                    AddEditExtendedAttributeModel.Text = null;
                    AddEditExtendedAttributeModel.Json = null;
                    break;
                case EntityExtendedAttributeType.Json:
                    AddEditExtendedAttributeModel.Decimal = null;
                    AddEditExtendedAttributeModel.Text = null;
                    AddEditExtendedAttributeModel.DateTime = null;
                    break;
            }

            var response = await ExtendedAttributeManager.SaveAsync(AddEditExtendedAttributeModel);
            if (_errorService.IsSuccessFull(response))
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            await HubConnection.TrySendAsync(_config.BackendOrigin, nameof(ClientEventHub.UpdateDashboardAsync));
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        }

        private async Task LoadDataAsync()
        {
            if (AddEditExtendedAttributeModel.DateTime.HasValue)
            {
                var date = AddEditExtendedAttributeModel.DateTime.Value;
                var time = TimeOnly.FromDateTime(date);
                _time = time.ToTimeSpan();
                _date = DateOnly.FromDateTime(date).ToDateTime(time);
            }

            await Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}