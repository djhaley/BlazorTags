using BlazorTags.State.Interfaces;
using BlazorTags.State.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BlazorTags.State.Forms
{
    public class StateForm<TState, TReducer> : ComponentBase
        where TReducer : IReducer<TState>, new()
    {
        private readonly Func<Task> _handleSubmitDelegate;

        private StateContext<TState, TReducer> _stateContext;

        public StateForm()
        {
            _handleSubmitDelegate = HandleSubmitAsync;
        }

        [Parameter(CaptureUnmatchedValues = true)] 
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [Parameter]
        public StateContext<TState, TReducer> StateContext
        {
            get => _stateContext;
            set
            {
                if (_stateContext != null)
                    throw new InvalidOperationException("StateContext can not be set more than once");

                if (value != null)
                {
                    _stateContext = value;
                    _stateContext.StateChanged += _stateContext_StateChanged;
                }
            }
        }

        private void _stateContext_StateChanged(object sender, StateChangedEventArgs e)
        {
            StateHasChanged();
        }

        [Parameter] 
        public RenderFragment<StateContext<TState, TReducer>> ChildContent { get; set; }

        [Parameter] 
        public EventCallback<StateContext<TState, TReducer>> OnSubmit { get; set; }

        [Parameter] 
        public EventCallback<StateContext<TState, TReducer>> OnValidSubmit { get; set; }

        [Parameter] 
        public EventCallback<StateContext<TState, TReducer>> OnInvalidSubmit { get; set; }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (_stateContext == null)
            {
                throw new InvalidOperationException($"{nameof(StateForm<TState, TReducer>)} requires " +
                    $"{nameof(StateContext<TState, TReducer>)} to be provided.");
            }

            // If you're using OnSubmit, it becomes your responsibility to trigger validation manually
            // (e.g., so you can display a "pending" state in the UI). In that case you don't want the
            // system to trigger a second validation implicitly, so don't combine it with the simplified
            // OnValidSubmit/OnInvalidSubmit handlers.
            if (OnSubmit.HasDelegate && (OnValidSubmit.HasDelegate || OnInvalidSubmit.HasDelegate))
            {
                throw new InvalidOperationException($"When supplying an {nameof(OnSubmit)} parameter to " +
                    $"{nameof(StateForm<TState, TReducer>)}, do not also supply {nameof(OnValidSubmit)} or {nameof(OnInvalidSubmit)}.");
            }
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            Debug.Assert(_stateContext != null);

            // If _editContext changes, tear down and recreate all descendants.
            // This is so we can safely use the IsFixed optimization on CascadingValue,
            // optimizing for the common case where _editContext never changes.
            builder.OpenRegion(_stateContext.GetHashCode());

            builder.OpenElement(0, "form");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "onsubmit", _handleSubmitDelegate);
            builder.OpenComponent<CascadingValue<IStateContext>>(3);
            builder.AddAttribute(4, "IsFixed", true);
            builder.AddAttribute(5, "Value", _stateContext);
            builder.AddAttribute(6, "ChildContent", ChildContent?.Invoke(_stateContext));
            builder.CloseComponent();
            builder.CloseElement();

            builder.CloseRegion();
        }

        private async Task HandleSubmitAsync()
        {
            Debug.Assert(_stateContext != null);

            if (OnSubmit.HasDelegate)
            {
                // When using OnSubmit, the developer takes control of the validation lifecycle
                await OnSubmit.InvokeAsync(_stateContext);
            }
            else
            {
                // Otherwise, the system implicitly runs validation on form submission
                var isValid = _stateContext.Validate();

                if (isValid && OnValidSubmit.HasDelegate)
                {
                    await OnValidSubmit.InvokeAsync(_stateContext);
                }

                if (!isValid && OnInvalidSubmit.HasDelegate)
                {
                    await OnInvalidSubmit.InvokeAsync(_stateContext);
                }
            }
        }
    }
}
