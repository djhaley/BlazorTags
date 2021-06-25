using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BlazorTags.State.Forms
{
    public static class StateInputBaseExtensions
    {
        public static bool TryParseSelectableValueFromString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(
               this StateInputBase<TValue> input, 
               string value,
               [MaybeNullWhen(false)] out TValue result)
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
                throw new InvalidOperationException($"{input.GetType()} does not support the type '{typeof(TValue)}'.", ex);
            }
        }
    }
}
