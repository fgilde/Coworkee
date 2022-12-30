using System;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Nextended.Core.Extensions;

namespace Coworkee.Client.Shared.Components
{
    public partial class SimplePropertyEdit
    {
        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public object ValueBindInstance { get; set; }
        
        [Parameter]
        public PropertyInfo Property { get; set; }

        public string StringValue
        {
            get => Property?.GetValue(ValueBindInstance)?.MapTo<string>();
            set => Property?.SetValue(ValueBindInstance, value?.MapTo(Property?.PropertyType));
        }

        public int IntValue
        {
            get => Property?.GetValue(ValueBindInstance)?.MapTo<int>() ?? default(int);
            set => Property?.SetValue(ValueBindInstance, value.MapTo(Property?.PropertyType));
        }

        public decimal DecimalValue
        {
            get => Property?.GetValue(ValueBindInstance)?.MapTo<decimal>() ?? default(decimal);
            set => Property?.SetValue(ValueBindInstance, value.MapTo(Property?.PropertyType));
        }

        public double DoubleValue
        {
            get => Property?.GetValue(ValueBindInstance)?.MapTo<double>() ?? default(double);
            set => Property?.SetValue(ValueBindInstance, value.MapTo(Property?.PropertyType));
        }

        public float FloatValue
        {
            get => Property?.GetValue(ValueBindInstance)?.MapTo<float>() ?? default(float);
            set => Property?.SetValue(ValueBindInstance, value.MapTo(Property?.PropertyType));
        }

        public DateTime DateTimeValue
        {
            get => Property?.GetValue(ValueBindInstance)?.MapTo<DateTime>() ?? default(DateTime);
            set => Property?.SetValue(ValueBindInstance, value.MapTo(Property?.PropertyType));
        }
    }
}