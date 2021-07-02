using System;

namespace BlazorTags.Samples.Models
{
    public record Model(
        string Name, 
        int Age, 
        bool ShowNestedForm,         
        int FieldsUpdated,
        NestedModel Nested
    );

    public record NestedModel(
        DateTime Date,
        string RgbChoice,
        string Selection,
        string Description,
        string AgeRange,
        string Color
    );
}
