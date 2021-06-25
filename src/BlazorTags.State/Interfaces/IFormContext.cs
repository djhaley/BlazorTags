namespace BlazorTags.State.Interfaces
{
    public interface IFormContext
    {
        // StateContext methods needed by input components - not aware of TState

        bool TryGetPropertyData(object model, string propertyName, out PropertyData propertyData);
        PropertyData GetPropertyData(object model, string propertyName);
        void Dispatch(IStateAction action);
        void NotifyOfStateChange();
        void RegisterFormField(PropertyData propertyData);
        string GetValidationMessage(object model, string propertyName);
    }
}
