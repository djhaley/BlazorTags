// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputDate.cs, with the exception of inheriting from StateInputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputDate.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BlazorTags.State.Forms
{
    public class StateInputDate<TValue, TModel> : StateInputBase<TValue, TModel>
    {
        private const string DateFormat = "yyyy-MM-dd";

        [DisallowNull] 
        public ElementReference Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "type", "date");
            builder.AddAttribute(3, "class", CssClass);
            builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddElementReferenceCapture(6, __inputReference => Element = __inputReference);
            builder.CloseElement();
        }
        protected override string FormatValueAsString(TValue value)
        {
            switch (value)
            {
                case DateTime dateTimeValue:
                    return BindConverter.FormatValue(dateTimeValue, DateFormat, CultureInfo.InvariantCulture);
                case DateTimeOffset dateTimeOffsetValue:
                    return BindConverter.FormatValue(dateTimeOffsetValue, DateFormat, CultureInfo.InvariantCulture);
                default:
                    return string.Empty;
            }
        }

        protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result)
        {
            // Unwrap nullable types. We don't have to deal with receiving empty values for nullable
            // types here, because the underlying InputBase already covers that.
            var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

            bool success;
            if (targetType == typeof(DateTime))
            {
                success = TryParseDateTime(value, out result);
            }
            else if (targetType == typeof(DateTimeOffset))
            {
                success = TryParseDateTimeOffset(value, out result);
            }
            else
            {
                throw new InvalidOperationException($"The type '{targetType}' is not a supported date type.");
            }

            if (success)
            {
                Debug.Assert(result != null);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool TryParseDateTime(string value, [MaybeNullWhen(false)] out TValue result)
        {
            var success = BindConverter.TryConvertToDateTime(value, CultureInfo.InvariantCulture, DateFormat, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        private static bool TryParseDateTimeOffset(string value, [MaybeNullWhen(false)] out TValue result)
        {
            var success = BindConverter.TryConvertToDateTimeOffset(value, CultureInfo.InvariantCulture, DateFormat, out var parsedValue);
            if (success)
            {
                result = (TValue)(object)parsedValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}