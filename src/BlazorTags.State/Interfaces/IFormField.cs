namespace BlazorTags.State.Interfaces
{
    public interface IFormField
    {
        bool IsValid { get; set; }
        string ValidationMessage { get; set; }
        bool IsModified { get; }
    }
}
