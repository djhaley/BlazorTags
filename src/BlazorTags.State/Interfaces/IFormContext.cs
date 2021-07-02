namespace BlazorTags.State.Interfaces
{
    public interface IFormContext
    {
        // StateContext methods needed by input components - not aware of TState

        bool TryGetPropertyData(object model, string propertyName, out IPropertyData propertyData);
        IPropertyData GetPropertyData(object model, string propertyName);
        void Dispatch(IStateAction action);
        void NotifyOfStateChange();
        void RegisterFormField(IPropertyData propertyData);
        string GetValidationMessage(object model, string propertyName);
    }
}
