namespace BlazorTags.State.Interfaces
{
    public interface IFormContext
    {
        // StateContext methods needed by input components - not aware of TState

        bool TryGetFormField(string id, out IFormField formField);
        IFormField GetFormField(string id);
        void Dispatch(IStateAction action);
        void NotifyOfStateChange();
        string GetValidationMessage(string id);
        void RegisterFormField(string id, IFormField formField);
    }
}
