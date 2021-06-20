using BlazorTags.State.Interfaces;
using System;

namespace BlazorTags.State
{
    public class StateChangedEventArgs : EventArgs
    {
        public IStateAction TriggeringAction { get; set; }
    }
}
