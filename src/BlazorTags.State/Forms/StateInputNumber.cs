// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputNumber.cs, with the exception of inheriting from StateInputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputNumber.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BlazorTags.State.Forms
{
    public class StateInputNumber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue, TModel> : StateInputBase<TValue, TModel>
    {
        private readonly static string _stepAttributeValue;

        static StateInputNumber()
        {
            // Unwrap Nullable<T>, because InputBase already deals with the Nullable aspect
            // of it for us. We will only get asked to parse the T for nonempty inputs.
            var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            if (targetType == typeof(int) ||
                targetType == typeof(long) ||
                targetType == typeof(short) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal))
            {
                _stepAttributeValue = "any";
            }
            else
            {
                throw new InvalidOperationException($"The type '{targetType}' is not a supported numeric type.");
            }
        }

        [DisallowNull] 
        public ElementReference Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "step", _stepAttributeValue);
            builder.AddMultipleAttributes(2, AdditionalAttributes);
            builder.AddAttribute(3, "type", "number");
            builder.AddAttribute(4, "class", CssClass);
            builder.AddAttribute(5, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(6, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddElementReferenceCapture(7, __inputReference => Element = __inputReference);
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result)
            => BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result);

        protected override string FormatValueAsString(TValue value)
        {
            // Avoiding a cast to IFormattable to avoid boxing.
            switch (value)
            {
                case null:
                    return null;

                case int @int:
                    return BindConverter.FormatValue(@int, CultureInfo.InvariantCulture);

                case long @long:
                    return BindConverter.FormatValue(@long, CultureInfo.InvariantCulture);

                case short @short:
                    return BindConverter.FormatValue(@short, CultureInfo.InvariantCulture);

                case float @float:
                    return BindConverter.FormatValue(@float, CultureInfo.InvariantCulture);

                case double @double:
                    return BindConverter.FormatValue(@double, CultureInfo.InvariantCulture);

                case decimal @decimal:
                    return BindConverter.FormatValue(@decimal, CultureInfo.InvariantCulture);

                default:
                    throw new InvalidOperationException($"Unsupported type {value.GetType()}");
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }
    }
}