namespace BlazorTags.State.Interfaces
{
    public interface IReducer<TState>
    {
        void Reduce(TState state, IStateAction action, IFormContext stateContext);
    }
}
