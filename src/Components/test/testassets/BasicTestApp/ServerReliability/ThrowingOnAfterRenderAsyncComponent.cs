// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BasicTestApp.ServerReliability
{
    public class ThrowingOnAfterRenderAsyncComponent : IComponent, IHandleAfterRender
    {
        public void Attach(RenderHandle renderHandle)
        {
            renderHandle.Render(builder =>
            {
                // Do nothing.
            });
        }

        public async Task OnAfterRenderAsync()
        {
            await Task.Yield();
            throw new InvalidTimeZoneException();
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            return Task.CompletedTask;
        }
    }
}
