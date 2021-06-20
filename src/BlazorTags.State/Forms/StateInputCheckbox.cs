// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputCheckbox.cs, with the exception of inheriting from StateInputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputCheckbox.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BlazorTags.State.Forms
{
    public class StateInputCheckbox : StateInputBase<bool>
    {
        [DisallowNull] 
        public ElementReference Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "type", "checkbox");
            builder.AddAttribute(3, "class", CssClass);
            builder.AddAttribute(4, "checked", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<bool>(this, __value => CurrentValue = __value, CurrentValue));
            builder.AddElementReferenceCapture(6, __inputReference => Element = __inputReference);
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string value, out bool result)
        => throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }
}
