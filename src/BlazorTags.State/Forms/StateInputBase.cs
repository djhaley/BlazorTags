﻿using BlazorTags.State.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorTags.State.Forms
{
    public abstract class StateInputBase<TValue, TModel> : ComponentBase
    {
        private Type _nullableUnderlyingType;
        private PropertyInfo _modelProperty;

        [CascadingParameter] 
        IStateContext CascadedStateContext { get; set; } = default!;

        [Parameter(CaptureUnmatchedValues = true)] 
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [Parameter]
        public TModel Model { get; set; }

        [Parameter]
        public string PropertyName { get; set; }

        [Parameter]
        public Func<TValue, IStateAction> ActionCreator { get; set; }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (CascadedStateContext == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a cascading parameter " +
                    $"of type {nameof(IStateContext)}. {GetType().FullName} should always be used inside " +
                    $"a StateForm.");
            }

            if (Model == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a value for the 'Model' parameter.");
            }

            if (PropertyName == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a value for the 'PropertyName' parameter.");
            }

            FieldIdentifier = new FieldIdentifier(Model, PropertyName);
            _nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TValue));

            UpdateAdditionalValidationAttributes();

            // For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
            return base.SetParametersAsync(ParameterView.Empty);
        }

        protected FieldIdentifier FieldIdentifier { get; set; }

        protected TValue CurrentValue
        {
            get => GetModelPropertyValue();
            set
            {
                var currentValue = GetModelPropertyValue();
                var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, currentValue);
                if (hasChanged)
                {
                    CascadedStateContext.Dispatch(ActionCreator(value));
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
                else if (TryParseValueFromString(value, out var parsedValue, out var validationErrorMessage))
                {
                    CurrentValue = parsedValue!;
                }
                else
                {
                    // Since we're not writing to CurrentValue, we'll need to notify about modification from here
                    CascadedStateContext.NotifyOfInvalidField(FieldIdentifier);
                }
            }
        }

        protected virtual string FormatValueAsString(TValue value) => value.ToString();

        protected abstract bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage);

        protected string CssClass
        {
            get
            {
                if (AdditionalAttributes != null &&
                    AdditionalAttributes.TryGetValue("class", out var @class) &&
                    !string.IsNullOrEmpty(Convert.ToString(@class, CultureInfo.InvariantCulture)))
                {
                    return $"{@class} {FieldClass}";
                }

                return FieldClass; // Never null or empty
            }
        }
        private string FieldClass => CascadedStateContext.FieldCssClass(FieldIdentifier);

        private void UpdateAdditionalValidationAttributes()
        {
            var hasAriaInvalidAttribute = AdditionalAttributes != null && AdditionalAttributes.ContainsKey("aria-invalid");
            if (!CascadedStateContext.IsValid(FieldIdentifier))
            {
                if (hasAriaInvalidAttribute)
                {
                    // Do not overwrite the attribute value
                    return;
                }

                if (ConvertToDictionary(AdditionalAttributes, out var additionalAttributes))
                {
                    AdditionalAttributes = additionalAttributes;
                }

                // To make the `Input` components accessible by default
                // we will automatically render the `aria-invalid` attribute when the validation fails
                additionalAttributes["aria-invalid"] = true;
            }
            else if (hasAriaInvalidAttribute)
            {
                // No validation errors. Need to remove `aria-invalid` if it was rendered already
                if (AdditionalAttributes!.Count == 1)
                {
                    // Only aria-invalid argument is present which we don't need any more
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
                _modelProperty = Model.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            }

            return (TValue)_modelProperty.GetValue(Model);
        }
    }
}