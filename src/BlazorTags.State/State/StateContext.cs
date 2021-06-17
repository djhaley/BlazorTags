using BlazorTags.State.Interfaces;
using System;
using System.Linq;

namespace BlazorTags.State.State
{
    public class StateContext<TState> : IActionDispatcher
    {
        private TState _state;

        private readonly Func<TState, IStateAction, TState> _rootReducer;

        public StateContext(TState initialState, Func<TState, IStateAction, TState> rootReducer)
        {
            _state = initialState;
            _rootReducer = rootReducer;
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public TState State { get => _state; }

        public void Dispatch(IStateAction action)
        {
            _state = _rootReducer(_state, action);

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
