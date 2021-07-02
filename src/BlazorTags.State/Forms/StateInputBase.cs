// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the based on InputBase
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputBase.cs

using BlazorTags.State.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorTags.State.Forms
{
    public abstract class StateInputBase<TValue> : ComponentBase
    {
        private Type _nullableUnderlyingType;
        private PropertyInfo _modelProperty;

        [CascadingParameter] 
        IFormContext CascadedFormContext { get; set; } = default!;

        [Parameter(CaptureUnmatchedValues = true)] 
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public Func<TValue, IStateAction> ActionCreator { get; set; }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (CascadedFormContext == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a cascading parameter " +
                    $"of type {nameof(IFormContext)}. {GetType().FullName} should always be used inside " +
                    $"a StateForm.");
            }

            if (ValueExpression == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a value for the 'Expression' parameter.");
            }

            var expressionPropertyData = new PropertyData<TValue>(ValueExpression);
            if (CascadedFormContext.TryGetPropertyData(expressionPropertyData.Model, expressionPropertyData.PropertyName, out IPropertyData propertyData))
            {
                PropertyData = propertyData;
            }
            else
            {
                PropertyData = expressionPropertyData;
                CascadedFormContext.RegisterFormField(PropertyData);
            }

            _nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TValue));

            UpdateAdditionalValidationAttributes();

            // For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
            return base.SetParametersAsync(ParameterView.Empty);
        }

        protected IPropertyData PropertyData { get; set; }

        protected TValue CurrentValue
        {
            get => GetModelPropertyValue();
            set
            {
                var currentValue = GetModelPropertyValue();
                var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, currentValue);
                if (hasChanged)
                {
                    CascadedFormContext.Dispatch(ActionCreator(value));
                }
            }
        }

        protected string CurrentValueAsString
        {
            get => FormatValueAsString(CurrentValue);
            set
            {
                if (_nullableUnderlyingType != null && string.IsNullOrEmpty(value))
                {
                    // Assume if it's a nullable type, null/empty inputs should correspond to default(T)
                    // Then all subclasses get nullable support almost automatically (they just have to
                    // not reject Nullable<T> based on the type itself).
                    CurrentValue = default!;
                }
                else if (TryParseValueFromString(value, out var parsedValue))
                {
                    CurrentValue = parsedValue!;
                }
                else
                {
                    // We couldn't parse the value, so we need to flag the field as invalid and let the context know
                    PropertyData.IsValid = false;
                    CascadedFormContext.NotifyOfStateChange();
                }
            }
        }

        protected virtual string FormatValueAsString(TValue value) => value.ToString();

        protected abstract bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result);

        protected string CssClass
        {
            get
            {
                if (AdditionalAttributes != null &&
                    AdditionalAttributes.TryGetValue("class", out var @class) &&
                    !string.IsNullOrEmpty(Convert.ToString(@class, CultureInfo.InvariantCulture)))
                {
                    return $"{@class} {PropertyData.CssClass}";
                }

                return PropertyData.CssClass;
            }
        }

        private void UpdateAdditionalValidationAttributes()
        {
            var hasAriaInvalidAttribute = AdditionalAttributes != null && AdditionalAttributes.ContainsKey("aria-invalid");
            if (PropertyData.IsValid)
            {
                if (hasAriaInvalidAttribute)
                {
                    return;
                }

                if (ConvertToDictionary(AdditionalAttributes, out var additionalAttributes))
                {
                    AdditionalAttributes = additionalAttributes;
                }

                additionalAttributes["aria-invalid"] = true;
            }
            else if (hasAriaInvalidAttribute)
            {
                if (AdditionalAttributes!.Count == 1)
                {
                    AdditionalAttributes = null;
                }
                else
                {
                    if (ConvertToDictionary(AdditionalAttributes, out var additionalAttributes))
                    {
                        AdditionalAttributes = additionalAttributes;
                    }

                    additionalAttributes.Remove("aria-invalid");
                }
            }
        }

        private bool ConvertToDictionary(IReadOnlyDictionary<string, object> source, out Dictionary<string, object> result)
        {
            var newDictionaryCreated = true;
            if (source == null)
            {
                result = new Dictionary<string, object>();
            }
            else if (source is Dictionary<string, object> currentDictionary)
            {
                result = currentDictionary;
                newDictionaryCreated = false;
            }
            else
            {
                result = new Dictionary<string, object>();
                foreach (var item in source)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return newDictionaryCreated;
        }

        private TValue GetModelPropertyValue()
        {
            if (_modelProperty == null)
            {
                _modelProperty = PropertyData.Model.GetType().GetProperty(PropertyData.PropertyName, BindingFlags.Public | BindingFlags.Instance);
            }

            return (TValue)_modelProperty.GetValue(PropertyData.Model);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            ParseAccessor(ValueExpression, out object model, out string propertyName);
            if (propertyName == "Selection") Console.WriteLine($"on after render: {model}");

            base.OnAfterRender(firstRender);
        }

        private static void ParseAccessor<T>(Expression<Func<T>> accessor, out object model, out string propertyName)
        {
            var accessorBody = accessor.Body;

            // Unwrap casts to object
            if (accessorBody is UnaryExpression unaryExpression
                && unaryExpression.NodeType == ExpressionType.Convert
                && unaryExpression.Type == typeof(object))
            {
                accessorBody = unaryExpression.Operand;
            }

            if (!(accessorBody is MemberExpression memberExpression))
            {
                throw new ArgumentException($"Oops");
            }

            // Identify the property name. We don't mind whether it's a property or field, or even something else.
            propertyName = memberExpression.Member.Name;

            // Get a reference to the model object
            // i.e., given an value like "(something).MemberName", determine the runtime value of "(something)",
            if (memberExpression.Expression is ConstantExpression constantExpression)
            {
                if (constantExpression.Value is null)
                {
                    throw new ArgumentException("The provided expression must evaluate to a non-null value.");
                }
                model = constantExpression.Value;
            }
            else if (memberExpression.Expression != null)
            {
                // It would be great to cache this somehow, but it's unclear there's a reasonable way to do
                // so, given that it embeds captured values such as "this". We could consider special-casing
                // for "() => something.Member" and building a cache keyed by "something.GetType()" with values
                // of type Func<object, object> so we can cheaply map from "something" to "something.Member".
                var modelLambda = Expression.Lambda(memberExpression.Expression);
                var modelLambdaCompiled = (Func<object>)modelLambda.Compile();
                var result = modelLambdaCompiled();
                if (result is null)
                {
                    throw new ArgumentException("The provided expression must evaluate to a non-null value.");
                }
                model = result;
            }
            else
            {
                throw new ArgumentException($"Ooops");
            }
        }

    }
}