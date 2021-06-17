using BlazorTags.State.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTags.State
{
    public class DefaultStateInputAction<TValue> : IStateInputAction<TValue>
    {
        public TValue Value { get; set; }
        public FieldIdentifier FieldIdentifier { get; set; }
    }
}
