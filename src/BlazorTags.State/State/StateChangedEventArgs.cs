using BlazorTags.State.Interfaces;
using System;

namespace BlazorTags.State.State
{
    public class StateChangedEventArgs : EventArgs
    {
        public IStateAction TriggeringAction { get; set; }
    }
}
