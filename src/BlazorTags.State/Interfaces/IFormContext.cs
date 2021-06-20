using System.Collections.Generic;

namespace BlazorTags.State.Interfaces
{
    public interface IFormContext
    {
        bool TryGetPropertyData(object model, string propertyName, out PropertyData propertyData);
        PropertyData GetPropertyData(object model, string propertyName);
        void Dispatch(IStateAction action);
        void NotifyOfStateChange(List<PropertyData> propertyDataChanges);
        void RegisterFormField(PropertyData propertyData);
    }
}
