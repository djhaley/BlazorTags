# BlazorTags

## BlazorTags.State
BlazorTags.State is an easy-to-use replacement for EditForm, EditContext, and the Input* components that will provide Redux style state management to your Blazor applications. 

### Usage
- Add the BlazorTags.State NuGet package.
- Add the following namespaces to your _Imports.razor

```csharp
@using BlazorTags.State
@using BlazorTags.State.Forms
@using BlazorTags.State.Interfaces
```
- Create your model, actions, and reducers, and be on your way!

### Example
This is Counter.razor from BlazorTag.Samples, which is simply a new Blazor WebAssembly project with the changes mentioned above.
- StateInputText (and Number, Checkbox, etc) for the most part act like their counterparts in the Microsoft.AspNetCore.Components.Forms namespace.
- One change is that @bind-Value is not used since that is tightly tied to the two-way binding in EditContext. Instead, simply provide an expression that specifies the value bound to this input.
- In addition to ValueExpression, provide a Func\<IStateAction\> in the ActionCreator field that will be used when the value changes.
- Another change is that your reducer will also be responsible for validating your model.
```csharp
@page "/counter"

<StateForm StateContext="stateContext" TState="Model" TReducer="Reducer">
    <h1>Counter</h1>

    <p>Field Updates: @stateContext.State.FieldsUpdated</p>

    <StateInputText ValueExpression="() => stateContext.State.Name"
                    ActionCreator="value => new UpdateNameAction(value)"></StateInputText>

    <StateInputNumber ValueExpression="() => stateContext.State.Age"
                      ActionCreator="value => new UpdateAgeAction(value)"></StateInputNumber>
</StateForm>

@code {

    private StateContext<Model, Reducer> stateContext;

    protected override void OnInitialized()
    {
        var model = new Model("Dan", 29, 0);
        stateContext = new StateContext<Model, Reducer>(model);

        base.OnInitialized();
    }



    private record Model(string Name, int Age, int FieldsUpdated);

    private class Reducer : IReducer<Model>
    {
        public Model Reduce(Model state, IStateAction action)
        {
            switch (action)
            {
                case UpdateNameAction a:
                    return state with { Name = a.NewName, FieldsUpdated = state.FieldsUpdated + 1 };
                case UpdateAgeAction a:
                    return state with { Age = a.NewAge, FieldsUpdated = state.FieldsUpdated + 1 };
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
        }
    }

    private record UpdateNameAction(string NewName) : IStateAction;
    private record UpdateAgeAction(int NewAge) : IStateAction;
}
```