namespace BlazorTags.State.Interfaces
{
    public interface IFormContext
    {
        bool TryGetPropertyData(object model, string propertyName, out PropertyData propertyData);
        void Dispatch(IStateAction action);
        void NotifyOfStateChange();
        void RegisterFormField(PropertyData propertyData);
    }
}
