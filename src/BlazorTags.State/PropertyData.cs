﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the based on FieldIdentifier
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Forms/src/FieldIdentifier.cs

using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BlazorTags.State
{
    public class PropertyData : IEquatable<PropertyData>
    {
        private object _model;
        private PropertyInfo _propertyInfo;
        private string _originalValueAsJson;

        public static PropertyData Create<TField>(Expression<Func<TField>> accessor)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));

            ParseAccessor(accessor, out object model, out string propertyName);
            return new PropertyData(model, propertyName);
        }

        public PropertyData(object model, string propertyName)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.GetType().IsValueType) throw new ArgumentException("The model must be a reference-typed object.", nameof(model));

            _model = model;
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

            _propertyInfo = _model.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            _originalValueAsJson = JsonConvert.SerializeObject(_propertyInfo.GetValue(_model));
        }

        public object Model 
        {
            get => _model;
            set
            {
                if (!value.GetType().Equals(_model.GetType()))
                    throw new InvalidOperationException($"Updating the {nameof(Model)} property can only be done with the same type of object.");

                _model = value;
            }
        }

        public string PropertyName { get; }
        public bool IsValid { get; set; } = true;
        public string CssClass { get => (IsModified() ? "modified " : "") + (IsValid ? "valid" : "invalid"); }

        public bool IsModified() 
        {
            var currentValueAsJson = JsonConvert.SerializeObject(_propertyInfo.GetValue(_model));
            return !string.Equals(currentValueAsJson, _originalValueAsJson, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            var modelHash = RuntimeHelpers.GetHashCode(_model);
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
            return ReferenceEquals(otherData.Model, _model) &&
                string.Equals(otherData.PropertyName, PropertyName, StringComparison.Ordinal);
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
                throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. {nameof(PropertyData)} only supports simple member accessors (fields, properties) of an object.");
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
                throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. {nameof(PropertyData)} only supports simple member accessors (fields, properties) of an object.");
            }
        }
    }
}
