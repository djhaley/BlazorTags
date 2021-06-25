// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ** This class is the same as InputRadioContext.cs
// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputRadioContext.cs

using Microsoft.AspNetCore.Components;

namespace BlazorTags.State.Forms
{
    public class StateInputRadioContext
    {
        private readonly StateInputRadioContext _parentContext;

        public string GroupName { get; }

        public object CurrentValue { get; }

        public string FieldClass { get; }

        public EventCallback<ChangeEventArgs> ChangeEventCallback { get; }

        public StateInputRadioContext(
            StateInputRadioContext parentContext,
            string groupName,
            object currentValue,
            string fieldClass,
            EventCallback<ChangeEventArgs> changeEventCallback
        )
        {
            _parentContext = parentContext;

            GroupName = groupName;
            CurrentValue = currentValue;
            FieldClass = fieldClass;
            ChangeEventCallback = changeEventCallback;
        }

        public StateInputRadioContext FindContextInAncestors(string groupName)
            => string.Equals(GroupName, groupName) ? this : _parentContext?.FindContextInAncestors(groupName);
    }
}