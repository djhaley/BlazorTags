namespace BlazorTags.State.Interfaces
{
    public interface IActionDispatcher
    {
        void Dispatch(IStateAction action);
    }
}
