using BlazorTags.Samples.Actions;
using BlazorTags.Samples.Models;
using BlazorTags.State;
using BlazorTags.State.Interfaces;

namespace BlazorTags.Samples.Reducers
{
    public class Reducer : IReducer<Model>
    {
        public Model Reduce(Model state, IStateAction action)
        {

            switch (action)
            {
                case UpdateNameAction a:
                    return state with { Name = a.NewName, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateAgeAction a:
                    return state with { Age = a.NewAge, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateAgeRangeAction a:
                    return state with { AgeRange = a.NewAgeRange, FieldsUpdated = state.FieldsUpdated + 1 };
                case ToggleShowNestedFormAction:
                    return state with { ShowNestedForm = !state.ShowNestedForm, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateDateAction a:
                    return state with { Date = a.NewDate, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateRgbChoiceAction a:
                    return state with { RgbChoice = a.Choice, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateDescriptionAction a:
                    return state with { Description = a.LongText, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateSelectionAction a:
                    return state with { Selection = a.Selection, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateColorAction a:
                    return state with { Color = a.Color, FieldsUpdated = state.FieldsUpdated + 1 };
                default:
                    return state;
            }
        }

        public void Validate(Model state, IFormContext formContext)
        {
            if (state.Age < 12 && formContext.TryGetPropertyData(state, "Age", out PropertyData ageData))
            {
                ageData.IsValid = false;
                ageData.ValidationMessage = "You must be 12 to ride this amusement park attraction";
            }

            if (state.Selection != "Washington" && formContext.TryGetPropertyData(state, "Selection", out PropertyData selectionData))
            {
                selectionData.IsValid = false;
                selectionData.ValidationMessage = "Washington!!!!!!!";
            }
        }
    }
}
