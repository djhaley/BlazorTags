using BlazorTags.Samples.Actions;
using BlazorTags.Samples.Models;
using BlazorTags.State.Interfaces;

namespace BlazorTags.Samples.Reducers
{
    public class NestedReducer : IReducer<NestedModel>
    {
        public NestedModel Reduce(NestedModel state, IStateAction action)
        {
            switch (action)
            {
                case UpdateAgeRangeAction a:
                    return state with { AgeRange = a.NewAgeRange };
                case UpdateDateAction a:
                    return state with { Date = a.NewDate };
                case UpdateRgbChoiceAction a:
                    return state with { RgbChoice = a.Choice };
                case UpdateDescriptionAction a:
                    return state with { Description = a.LongText };
                case UpdateSelectionAction a:
                    return state with { Selection = a.Selection };
                case UpdateColorAction a:
                    return state with { Color = a.Color };
                default:
                    return state;
            }
        }

        public void Validate(NestedModel state, IFormContext formContext)
        {
            if (state.Selection != "Washington" && formContext.TryGetFormField("stateInput", out IFormField selectionData))
            {
                selectionData.IsValid = false;
                selectionData.ValidationMessage = "Washington!!!!!!!";
            }
        }
    }
}
