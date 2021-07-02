namespace BlazorTags.State.Interfaces
{
    public interface IPropertyData
    {
        object Model { get; }
        string PropertyName { get; }
        bool IsValid { get; set; }
        string ValidationMessage { get; set; }
        string CssClass { get; }
        bool IsModified { get; }

        void UpdateModel();        
    }
}
