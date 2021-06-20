using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace BlazorTags.State
{
    public class ReducedStateInfo<TState>
    {
        public TState State { get; set; }
        public List<FieldIdentifier> InvalidFields { get; set; }
    }
}
