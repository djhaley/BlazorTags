using System;

namespace BlazorTags.State.Interfaces
{
    public interface IStateContext<TState>
    {
        // State methods used by non-Input components - TState aware

        event EventHandler<StateChangedEventArgs> StateChanged;
        event EventHandler<StateChangedEventArgs> ModelUpdated;

        TState State { get; }
        void Dispatch(IStateAction action);
        void NotifyOfStateChange();
    }
}
