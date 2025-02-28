using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sincere;

public partial class ViewerContainer : SincereComponentBase
{
    private int defaultZoomLevel = 8;

    private int maxZoomLevel = 17;

    private int minZoomLevel = 1;

    private DotNetObjectReference<ViewerContainer>? objRef;

    private Orientation? oldOrientation;

    private int pageNumber = 0;

    private int pagesCount = 0;

    private double rotation = 0;

    private double scale = 1.0;
    private int zoomLevel = 8;

    // TODO: numerize
    private string zoomPercentage = "100%";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await ViewerContainerJsInterop.InitializeAsync(objRef!, Id!, scale, rotation, Url!);

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        objRef ??= DotNetObjectReference.Create(this);

        rotation = Orientation == Orientation.Portrait ? 0 : -90;

        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (IsRenderComplete)
            if (!Orientation.Equals(oldOrientation))
            {
                oldOrientation = Orientation;
                rotation = Orientation == Orientation.Portrait ? 0 : -90;
                await ViewerContainerJsInterop.RotateAsync(objRef!, Id!, rotation);
            }

        await base.OnParametersSetAsync();
    }

    [JSInvokable]
    public void DocumentLoaded(ViewerModel ViewerModel)
    {
        if (ViewerModel is null) return;

        pageNumber = ViewerModel.PageNumber;
        pagesCount = ViewerModel.PagesCount;

        StateHasChanged();

        if (OnDocumentLoaded.HasDelegate)
            OnDocumentLoaded.InvokeAsync(new ViewerEventArgs(pageNumber, pagesCount));
    }

    [JSInvokable]
    public void SetViewerMetaData(ViewerModel ViewerModel)
    {
        if (ViewerModel is null) return;

        pageNumber = ViewerModel.PageNumber;
        pagesCount = ViewerModel.PagesCount;

        if (OnPageChanged.HasDelegate)
            OnPageChanged.InvokeAsync(new ViewerEventArgs(pageNumber, pagesCount));
    }

    private async Task FirstPageAsync() => await ViewerContainerJsInterop.FirstPageAsync(objRef!, Id!);

    private int GetZoomPercentage(int zoomLevel) =>
        zoomLevel switch
        {
            1 => 25,
            2 => 33,
            3 => 50,
            4 => 67,
            5 => 75,
            6 => 80,
            7 => 90,
            8 => 100,
            9 => 110,
            10 => 125,
            11 => 150,
            12 => 175,
            13 => 200,
            14 => 250,
            15 => 300,
            16 => 400,
            17 => 500,
            _ => 100
        };

    private async Task LastPageAsync() => await ViewerContainerJsInterop.LastPageAsync(objRef!, Id!);

    private async Task NextPageAsync() => await ViewerContainerJsInterop.NextPageAsync(objRef!, Id!);

    private async Task PageNumberChangedAsync(int value)
    {
        if (value < 1 || value > pagesCount)
            pageNumber = 1;
        else
            pageNumber = value;

        await ViewerContainerJsInterop.GotoPageAsync(objRef!, Id!, pageNumber);
    }

    private async Task PreviousPageAsync() => await ViewerContainerJsInterop.PreviousPageAsync(objRef!, Id!);

    private async Task PrintAsync() => await ViewerContainerJsInterop.PrintAsync(objRef!, Id!, Url!);

    private async Task ResetZoomAsync()
    {
        zoomLevel = defaultZoomLevel;
        var zp = GetZoomPercentage(defaultZoomLevel);
        zoomPercentage = $"{zp}%";
        scale = 0.01 * zp;
        await ViewerContainerJsInterop.ZoomInOutAsync(objRef!, Id!, scale);
    }

    private async Task RotateClockwiseAsync()
    {
        rotation += 90;
        rotation = rotation.Equals(360) ? 0 : rotation;
        await ViewerContainerJsInterop.RotateAsync(objRef!, Id!, rotation);

        // Orientation
        SetOrientation();
    }

    private async Task RotateCounterclockwiseAsync()
    {
        rotation -= 90;
        rotation = rotation.Equals(-360) ? 0 : rotation;
        await ViewerContainerJsInterop.RotateAsync(objRef!, Id!, rotation);

        // Orientation
        SetOrientation();
    }

    private void SetOrientation()
    {
        if (rotation == 0)
            oldOrientation = Orientation = Orientation.Portrait;
        else if (rotation == -90)
            oldOrientation = Orientation = Orientation.Landscape;
    }

    private async Task SwitchOrientationAsync()
    {
        oldOrientation = Orientation;
        Orientation = Orientation == Orientation.Portrait ? Orientation.Landscape : Orientation.Portrait;
        rotation = Orientation == Orientation.Portrait ? 0 : -90;
        await ViewerContainerJsInterop.RotateAsync(objRef!, Id!, rotation);
    }

    private async Task ZoomInAsync()
    {
        if (zoomLevel == maxZoomLevel)
            return;

        zoomLevel += 1;
        var zp = GetZoomPercentage(zoomLevel);
        zoomPercentage = $"{zp}%";
        scale = 0.01 * zp;
        await ViewerContainerJsInterop.ZoomInOutAsync(objRef!, Id!, scale);
    }

    private async Task ZoomOutAsync()
    {
        if (zoomLevel == minZoomLevel)
            return;

        zoomLevel -= 1;
        var zp = GetZoomPercentage(zoomLevel);
        zoomPercentage = $"{zp}%";
        scale = 0.01 * zp;
        await ViewerContainerJsInterop.ZoomInOutAsync(objRef!, Id!, scale);
    }


    /// <summary>
    /// This event fires immediately after the PDF document is loaded.
    /// </summary>
    [Parameter]
    public EventCallback<ViewerEventArgs> OnDocumentLoaded { get; set; }

    /// <summary>
    /// This event fires immediately after the page is changed.
    /// </summary>
    [Parameter]
    public EventCallback<ViewerEventArgs> OnPageChanged { get; set; }

    /// <summary>
    /// Gets or sets the preferred orientation for the PDF viewer.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="Orientation.Portrait" />.
    /// </remarks>
    [Parameter]
    public Orientation Orientation { get; set; } = Orientation.Portrait;

    /// <summary>
    /// Provides JavaScript interop functionality for the PDF viewer.
    /// </summary>
    [Inject]
    private ViewerContainerJsInterop ViewerContainerJsInterop { get; set; } = default!;

    /// <summary>
    /// Gets or sets the URL of the PDF document to be displayed.
    /// PDF Viewer component supports base64 string as a URL.
    /// </summary>
    /// <remarks>
    /// Default value is null.
    /// </remarks>
    [Parameter]
    public string? Url { get; set; }
}
