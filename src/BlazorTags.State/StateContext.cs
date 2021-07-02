// Copyright (c) .NET Foundation. All rights reserved.
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
        private Dictionary<string, IFormField> _formFields = new Dictionary<string, IFormField>();

        public StateContext(TState initialState)
        {
            _state = initialState;
            _rootReducer = new TReducer();
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public TState State { get => _state; }

        public bool TryGetFormField(string id, out IFormField formField)
        {
            formField = GetFormField(id);
            return formField != null;
        }

        public string GetValidationMessage(string id)
        {
            if (!TryGetFormField(id, out IFormField formField)) return "";
            return formField.ValidationMessage;
        }

        public IFormField GetFormField(string id)
        {
            if (!_formFields.ContainsKey(id)) return null;
            return _formFields[id];
        }

        public void Dispatch(IStateAction action)
        {
            foreach (var field in _formFields)
            {
                field.Value.IsValid = true;
                field.Value.ValidationMessage = "";
            }

            _state = _rootReducer.Reduce(_state, action);
            _rootReducer.Validate(_state, this);

            NotifyOfStateChange();
        }

        public bool Validate()
        {
            _rootReducer.Validate(_state, this);
            return !_formFields.Any(field => !field.Value.IsValid);
        }

        public void NotifyOfStateChange()
        {
            OnStateChanged(new StateChangedEventArgs());
        }

        public void RegisterFormField(string id, IFormField formField)
        {
            if (!_formFields.ContainsKey(id))
                _formFields.Add(id, formField);
        }

        protected virtual void OnStateChanged(StateChangedEventArgs eventArgs)
        {
            var stateChangedHandler = StateChanged;
            if (stateChangedHandler != null)
                stateChangedHandler(this, eventArgs);
        }
    }
}
