// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the based on FieldIdentifier
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Forms/src/FieldIdentifier.cs

using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BlazorTags.State
{
    public class PropertyData : IEquatable<PropertyData>
    {
        private PropertyInfo _propertyInfo;
        private string _originalValueAsJson;

        public PropertyData(object model, string propertyName)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.GetType().IsValueType) throw new ArgumentException("The model must be a reference-typed object.", nameof(model));

            Model = model;
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

            _propertyInfo = Model.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            _originalValueAsJson = JsonConvert.SerializeObject(_propertyInfo.GetValue(Model));
        }

        public object Model { get; }
        public string PropertyName { get; }
        public bool IsValid { get; set; } = true;
        public string CssClass { get => (IsModified() ? "modified " : "") + (IsValid ? "valid" : "invalid"); }

        public bool IsModified() 
        {
            var currentValueAsJson = JsonConvert.SerializeObject(_propertyInfo.GetValue(Model));
            return !string.Equals(currentValueAsJson, _originalValueAsJson, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            var modelHash = RuntimeHelpers.GetHashCode(Model);
            var fieldHash = StringComparer.Ordinal.GetHashCode(PropertyName);

            return (
                modelHash,
                fieldHash
            )
            .GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is PropertyData otherData && Equals(otherData);

        /// <inheritdoc />
        public bool Equals(PropertyData otherData)
        {
            return ReferenceEquals(otherData.Model, Model) &&
                string.Equals(otherData.PropertyName, PropertyName, StringComparison.Ordinal);
        }
    }
}
