﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sincere;

public abstract class SincereLayoutComponentBase : LayoutComponentBase, IDisposable, IAsyncDisposable
{

    private bool isAsyncDisposed;

    private bool isDisposed;


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        IsRenderComplete = true;

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnInitialized()
    {
        Id ??= IdUtility.GetNextId();

        base.OnInitialized();
    }

    public static string BuildClassNames(params (string? cssClass, bool when)[] cssClassList)
    {
        var list = new HashSet<string>();

        if (cssClassList is not null && cssClassList.Any())
            foreach (var (cssClass, when) in cssClassList)
                if (!string.IsNullOrWhiteSpace(cssClass) && when)
                    list.Add(cssClass);

        if (list.Any())
            return string.Join(" ", list);

        return string.Empty;
    }

    public static string BuildClassNames(string? userDefinedCssClass, params (string? cssClass, bool when)[] cssClassList)
    {
        var list = new HashSet<string>();

        if (cssClassList is not null && cssClassList.Any())
            foreach (var (cssClass, when) in cssClassList)
                if (!string.IsNullOrWhiteSpace(cssClass) && when)
                    list.Add(cssClass);

        if (!string.IsNullOrWhiteSpace(userDefinedCssClass))
            list.Add(userDefinedCssClass.Trim());

        if (list.Any())
            return string.Join(" ", list);

        return string.Empty;
    }

    public static string BuildStyleNames(string? userDefinedCssStyle, params (string? cssStyle, bool when)[] cssStyleList)
    {
        var list = new HashSet<string>();

        if (cssStyleList is not null && cssStyleList.Any())
            foreach (var (cssStyle, when) in cssStyleList)
                if (!string.IsNullOrWhiteSpace(cssStyle) && when)
                    list.Add(cssStyle);

        if (!string.IsNullOrWhiteSpace(userDefinedCssStyle))
            list.Add(userDefinedCssStyle.Trim());

        if (list.Any())
            return string.Join(';', list);

        return string.Empty;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true).ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                // cleanup
            }

            isDisposed = true;
        }
    }

    protected virtual ValueTask DisposeAsyncCore(bool disposing)
    {
        if (!isAsyncDisposed)
        {
            if (disposing)
            {
                // cleanup
            }

            isAsyncDisposed = true;
        }

        return ValueTask.CompletedTask;
    }

    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = default!;

    [Parameter] public string? Class { get; set; }

    protected virtual string? ClassNames => Class;

    public ElementReference Element { get; set; }

    [Parameter] public string? Id { get; set; }

    protected bool IsRenderComplete { get; private set; }

    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public string? Style { get; set; }

    protected virtual string? StyleNames => Style;

    ~SincereLayoutComponentBase()
    {
        Dispose(false);
    }
}
