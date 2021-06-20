﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is based loosely on EditContext.cs - https://github.com/dotnet/aspnetcore/blob/main/src/Components/Forms/src/EditContext.cs

using BlazorTags.State.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorTags.State
{
    public class StateContext<TState, TReducer> : IFormContext, IStateContext<TState>
        where TReducer : IReducer<TState>, new()
    {
        private TState _state;
        private TReducer _rootReducer;

        // State itself may encompass much more than form fields, but for tracking what
        //  is displayed on a form we'll capture anything registered by an input
        private List<PropertyData> _formFields = new List<PropertyData>();

        public StateContext(TState initialState)
        {
            _state = initialState;
            _rootReducer = new TReducer();
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public TState State { get => _state; }

        public bool TryGetPropertyData(object model, string propertyName, out PropertyData propertyData)
        {
            propertyData = GetPropertyData(model, propertyName);
            return propertyData != null;
        }

        public string GetValidationMessage(object model, string propertyName)
        {
            if (!TryGetPropertyData(model, propertyName, out PropertyData propertyData)) return "";
            return propertyData.ValidationMessage;
        }

        public PropertyData GetPropertyData(object model, string propertyName)
        {
            return _formFields.SingleOrDefault(field => ReferenceEquals(model, field.Model) &&
                string.Equals(propertyName, field.PropertyName, StringComparison.Ordinal));
        }

        public void Dispatch(IStateAction action)
        {
            _state = _rootReducer.Reduce(_state, action);
            _formFields.ForEach(field => { field.Model = _state; field.IsValid = true; });

            _rootReducer.Validate(_state, this);

            NotifyOfStateChange();
        }

        public bool Validate() => !_formFields.Any(field => !field.IsValid);

        public void NotifyOfStateChange()
        {
            OnStateChanged(new StateChangedEventArgs());
        }

        public void RegisterFormField(PropertyData propertyData)
        {
            if (!_formFields.Contains(propertyData)) _formFields.Add(propertyData);
        }

        protected virtual void OnStateChanged(StateChangedEventArgs eventArgs)
        {
            var stateChangedHandler = StateChanged;
            if (stateChangedHandler != null)
                stateChangedHandler(this, eventArgs);
        }
    }
}
