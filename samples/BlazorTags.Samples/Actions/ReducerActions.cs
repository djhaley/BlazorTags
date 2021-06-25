using BlazorTags.State.Interfaces;
using System;

namespace BlazorTags.Samples.Actions
{
    public record UpdateNameAction(string NewName) : IStateAction;
    public record UpdateAgeAction(int NewAge) : IStateAction;
    public record UpdateAgeRangeAction(string NewAgeRange) : IStateAction;
    public record ToggleShowNestedFormAction() : IStateAction;
    public record UpdateDateAction(DateTime NewDate) : IStateAction;
    public record UpdateSelectionAction(string Selection) : IStateAction;
    public record UpdateDescriptionAction(string LongText) : IStateAction;
    public record UpdateRgbChoiceAction(string Choice) : IStateAction;
    public record UpdateColorAction(string Color) : IStateAction;
}
