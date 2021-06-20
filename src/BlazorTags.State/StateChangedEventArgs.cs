using System;
using System.Collections.Generic;

namespace BlazorTags.State
{
    public class StateChangedEventArgs : EventArgs
    {
        public List<PropertyData> ChangedProperties { get; set; }
    }
}
