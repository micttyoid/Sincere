using Microsoft.JSInterop;
namespace Sincere;

public class ViewerContainerJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public ViewerContainerJsInterop(IJSRuntime jsRuntime)
    {
        // workerSrc: https://mozilla.github.io/pdf.js/examples
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/LukeYoo.Blazor.Sincere/blazor.sincere.pdf.js").AsTask());
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    public async Task FirstPageAsync(object objRef, string elementId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("firstPage", objRef, elementId);
    }

    public async Task GotoPageAsync(object objRef, string elementId, int gotoPageNum)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("gotoPage", objRef, elementId, gotoPageNum);
    }

    public async Task InitializeAsync(object objRef, string elementId, double scale, double rotation, string url)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("initialize", objRef, elementId, scale, rotation, url);
    }

    public async Task LastPageAsync(object objRef, string elementId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("lastPage", objRef, elementId);
    }

    public async Task NextPageAsync(object objRef, string elementId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("nextPage", objRef, elementId);
    }

    public async Task PreviousPageAsync(object objRef, string elementId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("previousPage", objRef, elementId);
    }

    public async Task PrintAsync(object objRef, string elementId, string url)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("print", objRef, elementId, url);
    }

    public async Task RotateAsync(object objRef, string elementId, double rotation)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("rotate", objRef, elementId, rotation);
    }

    public async Task ZoomInOutAsync(object objRef, string elementId, double scale)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("zoomInOut", objRef, elementId, scale);
    }
}
