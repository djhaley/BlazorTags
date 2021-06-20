namespace BlazorTags.State.Interfaces
{
    public interface IStateContext<TState>
    {
        TState State { get; }
        void Dispatch(IStateAction action);
    }
}
