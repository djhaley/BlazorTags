// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputSelect.cs + InputExtensions.cs, with the exception of inheriting from StateInputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputSelect.cs
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputExtensions.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BlazorTags.State.Forms
{
    public class StateInputSelect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : StateInputBase<TValue>
    {
        [Parameter] 
        public RenderFragment ChildContent { get; set; }

        [DisallowNull] 
        public ElementReference Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "select");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddElementReferenceCapture(5, __selectReference => Element = __selectReference);
            builder.AddContent(6, ChildContent);
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result)
            => TryParseSelectableValueFromString(value, out result);

        private bool TryParseSelectableValueFromString(string value, [MaybeNullWhen(false)] out TValue result)
        {
            try
            {
                if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue))
                {
                    result = parsedValue;
                    return true;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"{GetType()} does not support the type '{typeof(TValue)}'.", ex);
            }
        }
    }
}