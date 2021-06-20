using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTags.State.Interfaces
{
    public interface IStateContext
    {
        bool Validate();
        bool TryGetPropertyData(object model, string propertyName, out PropertyData propertyData);
        void Dispatch(IStateAction action);
        void NotifyOfStateChange();
        void RegisterFormField(PropertyData propertyData);
    }
}
