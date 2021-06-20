using BlazorTags.State.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorTags.State
{
    public class StateContext<TState, TReducer> : IStateContext
        where TReducer : IReducer<TState>, new()
    {
        private TState _state;
        private TReducer _rootReducer;

        private List<FieldIdentifier> _invalidFields = new List<FieldIdentifier>();

        public StateContext(TState initialState)
        {
            _state = initialState;
            _rootReducer = new TReducer();
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public TState State { get => _state; }

        public void Dispatch(IStateAction action)
        {
            var reducedState = _rootReducer.Reduce(_state, action);
            
            _state = reducedState.State;
            _invalidFields = reducedState.InvalidFields;

            OnStateChanged(new StateChangedEventArgs());
        }

        public string FieldCssClass(FieldIdentifier fieldIdentifier)
        {
            if (_invalidFields.Contains(fieldIdentifier)) return "invalid";
            return "valid";
        }

        public bool IsValid(FieldIdentifier fieldIdentifier) => !_invalidFields.Contains(fieldIdentifier);

        public bool Validate() => !_invalidFields.Any();

        public void NotifyOfInvalidField(FieldIdentifier fieldIdentifier)
        {
            if (!_invalidFields.Contains(fieldIdentifier))
                _invalidFields.Add(fieldIdentifier);

            OnStateChanged(new StateChangedEventArgs());
        }

        protected virtual void OnStateChanged(StateChangedEventArgs eventArgs)
        {
            var stateChangedHandler = StateChanged;
            if (stateChangedHandler != null)
                stateChangedHandler(this, eventArgs);
        }
    }
}
