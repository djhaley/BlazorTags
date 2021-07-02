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
See the [example project](https://github.com/djhaley/BlazorTags/tree/main/samples/BlazorTags.Samples) located in the source repository.


### Known Issues
- Does not support data annotations

### Roadmap
**v1.0.0** (Will have breaking changes)
- <State> tag to allow using StateContext outside of a form
- Model validation using data annotations
- Refactored interfaces to make usage easier (*breaking change*)
- Documentation
- \+ Nested state in ValueExpression

**v0.3.0** (Not releasing due to breaking changes for nested state)
- ~~Nested state in ValueExpression~~
