using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTags.State.Interfaces
{
    public interface IStateContext
    {
        string FieldCssClass(FieldIdentifier fieldIdentifier);
        bool IsValid(FieldIdentifier fieldIdentifier);
        bool Validate();

        void Dispatch(IStateAction action);
        void NotifyOfInvalidField(FieldIdentifier fieldIdentifier);
    }
}
