// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputRadio.cs
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputRadio.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BlazorTags.State.Forms
{
    public class StateInputRadio<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : ComponentBase
    {
        internal StateInputRadioContext Context { get; private set; }

        [Parameter(CaptureUnmatchedValues = true)] 
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter] public string Name { get; set; }

        [CascadingParameter] 
        private StateInputRadioContext CascadedContext { get; set; }

        private string GetCssClass(string fieldClass)
        {
            if (AdditionalAttributes != null &&
                AdditionalAttributes.TryGetValue("class", out var @class) &&
                !string.IsNullOrEmpty(Convert.ToString(@class, CultureInfo.InvariantCulture)))
            {
                return $"{@class} {fieldClass}";
            }

            return fieldClass;
        }

        protected override void OnParametersSet()
        {
            Context = string.IsNullOrEmpty(Name) ? CascadedContext : CascadedContext?.FindContextInAncestors(Name);

            if (Context == null)
            {
                throw new InvalidOperationException($"{GetType()} must have an ancestor {typeof(StateInputRadioGroup<TValue>)} " +
                    $"with a matching 'Name' property, if specified.");
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            Debug.Assert(Context != null);

            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", GetCssClass(Context.FieldClass));
            builder.AddAttribute(3, "type", "radio");
            builder.AddAttribute(4, "name", Context.GroupName);
            builder.AddAttribute(5, "value", BindConverter.FormatValue(Value?.ToString()));
            builder.AddAttribute(6, "checked", Context.CurrentValue?.Equals(Value));
            builder.AddAttribute(7, "onchange", Context.ChangeEventCallback);
            builder.CloseElement();
        }
    }
}
