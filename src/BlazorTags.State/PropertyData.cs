// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the based on FieldIdentifier
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Forms/src/FieldIdentifier.cs

using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace BlazorTags.State
{
    public class PropertyData : IEquatable<PropertyData>
    {
        public PropertyData(object model, string propertyName)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.GetType().IsValueType) throw new ArgumentException("The model must be a reference-typed object.", nameof(model));

            Model = model;
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

            OriginalValue = JsonConvert
        }

        public object Model { get; }
        public string PropertyName { get; }
        public object OriginalValue { get; }

        public override int GetHashCode()
        {
            var modelHash = RuntimeHelpers.GetHashCode(Model);
            var fieldHash = StringComparer.Ordinal.GetHashCode(FieldName);
            return (
                modelHash,
                fieldHash
            )
            .GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is FieldIdentifier otherIdentifier
            && Equals(otherIdentifier);

        /// <inheritdoc />
        public bool Equals(FieldIdentifier otherIdentifier)
        {
            return
                ReferenceEquals(otherIdentifier.Model, Model) &&
                string.Equals(otherIdentifier.FieldName, FieldName, StringComparison.Ordinal);
        }
    }
}
