// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputRadioGroup.cs, with the exception of inheriting from StateInputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputRadioGroup.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BlazorTags.State.Forms
{
    public class StateInputRadioGroup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : StateInputBase<TValue>
    {
        private readonly string _defaultGroupName = Guid.NewGuid().ToString("N");
        private StateInputRadioContext _context;

        [Parameter] 
        public RenderFragment ChildContent { get; set; }

        [Parameter] 
        public string Name { get; set; }

        [CascadingParameter] 
        private StateInputRadioContext CascadedContext { get; set; }

        protected override void OnParametersSet()
        {
            var groupName = !string.IsNullOrEmpty(Name) ? Name : _defaultGroupName;
            var changeEventCallback = EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString);

            _context = new StateInputRadioContext(CascadedContext, groupName, CurrentValue, CssClass, changeEventCallback);
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            Debug.Assert(_context != null);

            builder.OpenComponent<CascadingValue<StateInputRadioContext>>(0);
            builder.SetKey(_context);
            builder.AddAttribute(1, "IsFixed", true);
            builder.AddAttribute(2, "Value", _context);
            builder.AddAttribute(3, "ChildContent", ChildContent);
            builder.CloseComponent();
        }

        /// <inheritdoc />
        protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result)
            => this.TryParseSelectableValueFromString(value, out result);
    }
}
