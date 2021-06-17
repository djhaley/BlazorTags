using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTags.State.Interfaces
{
    public interface IStateInputAction<TValue> : IStateAction
    {
        public TValue Value { get; set; }
        public FieldIdentifier FieldIdentifier { get; set; }
    }
}
