﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputText.cs, with the exception of inheriting from StateInputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputText.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BlazorTags.State.Forms
{
    public class StateInputText<TModel> : StateInputBase<string, TModel>
    {
        [DisallowNull] 
        public ElementReference Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddElementReferenceCapture(5, __inputReference => Element = __inputReference);
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string value, out string result)
        {
            result = value;
            return true;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }
    }
}