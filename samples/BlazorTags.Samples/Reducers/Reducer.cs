using BlazorTags.Samples.Actions;
using BlazorTags.Samples.Models;
using BlazorTags.State.Interfaces;

namespace BlazorTags.Samples.Reducers
{
    public class Reducer : IReducer<Model>
    {
        private readonly NestedReducer _nestedReducer = new NestedReducer();

        public Model Reduce(Model state, IStateAction action)
        {
            switch (action)
            {
                case UpdateNameAction a:
                    return state with { Name = a.NewName, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateAgeAction a:
                    return state with { Age = a.NewAge, FieldsUpdated = state.FieldsUpdated + 1 };
                case ToggleShowNestedFormAction:
                    return state with { ShowNestedForm = !state.ShowNestedForm, FieldsUpdated = state.FieldsUpdated + 1 };
                default:
                    return state with { Nested = _nestedReducer.Reduce(state.Nested, action) };
            }
        }

        public void Validate(Model state, IFormContext formContext)
        {
            _nestedReducer.Validate(state.Nested, formContext);

            if (state.Age < 12 && formContext.TryGetFormField("ageInput", out IFormField ageData))
            {
                ageData.IsValid = false;
                ageData.ValidationMessage = "You must be 12 to ride this amusement park attraction";
            }
        }
    }
}
