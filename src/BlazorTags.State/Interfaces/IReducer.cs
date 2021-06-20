namespace BlazorTags.State.Interfaces
{
    public interface IReducer<TState>
    {
        TState Reduce(TState state, IStateAction action);
        void Validate(TState state, IFormContext formContext);
    }
}
