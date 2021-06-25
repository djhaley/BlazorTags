using System;

namespace BlazorTags.Samples.Models
{
    public record Model(
        string Name, 
        int Age, 
        bool ShowNestedForm, 
        DateTime Date, 
        string RgbChoice, 
        string Selection,
        string Description, 
        int FieldsUpdated,
        string AgeRange,
        string Color
    );
}
