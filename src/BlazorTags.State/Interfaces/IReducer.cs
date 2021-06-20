namespace BlazorTags.State.Interfaces
{
    public interface IReducer<TState>
    {
        ReducedStateInfo<TState> Reduce(TState previousState, IStateAction action);
    }
}
