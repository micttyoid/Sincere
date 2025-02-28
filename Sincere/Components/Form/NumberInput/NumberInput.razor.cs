/* TODO: Should i remove it? unlike Boostrap, it's only uint/ulong for this application */

using Microsoft.AspNetCore.Components;

namespace Sincere;

// TODO: restrict TValue to uint, ulong
// TODO: restrict min 1
public partial class NumberInput<TValue> : SincereComponentBase
{

    private FieldIdentifier fieldIdentifier;

    private string step = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Try to do input type-check at the c#-end
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("window.sincere.numberInput.initialize", Id, isFloatingNumber(), AllowNegativeNumbers);

            var currentValue = Value; // object

            if (currentValue is null || !TryParseValue(currentValue, out var value))
                Value = default!;
            else if (EnableMinMax && Min is not null && IsLeftGreaterThanRight(Min, Value)) // value < min
                Value = Min;
            else if (EnableMinMax && Max is not null && IsLeftGreaterThanRight(Value, Max)) // value > max
                Value = Max;
            else
                Value = value;

            await ValueChanged.InvokeAsync(Value);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        if (IsLeftGreaterThanRight(Min, Max))
            throw new InvalidOperationException("The Min parameter value is greater than the Max parameter value.");

        if (!(
              typeof(TValue) == typeof(int)
              || typeof(TValue) == typeof(int?)
              || typeof(TValue) == typeof(long)
              || typeof(TValue) == typeof(long?)
             ))
            throw new InvalidOperationException($"{typeof(TValue)} is not supported.");

        AdditionalAttributes ??= new Dictionary<string, object>();

        fieldIdentifier = FieldIdentifier.Create(ValueExpression);

        step = Step.HasValue ? $"{Step.Value}" : "any";

        await base.OnInitializedAsync();
    }

    public void Disable() => Disabled = true;

    public void Enable() => Disabled = false;

    private string GetInvariantNumber(TValue value)
    {
        if (value is null) return string.Empty;
        // All numbers without decimal places work fine by default
        return value?.ToString() ?? string.Empty;
    }

    private bool isFloatingNumber() =>
        typeof(TValue) == typeof(float)
        || typeof(TValue) == typeof(float?)
        || typeof(TValue) == typeof(double)
        || typeof(TValue) == typeof(double?)
        || typeof(TValue) == typeof(decimal)
        || typeof(TValue) == typeof(decimal?);

    private bool IsLeftGreaterThanRight(TValue left, TValue right)
    {
        if (typeof(TValue) == typeof(int))
        {
            var l = Convert.ToInt32(left);
            var r = Convert.ToInt32(right);

            return l > r;
        }

        // int?

        if (typeof(TValue) == typeof(int?))
        {
            var l = left as int?;
            var r = right as int?;

            return l.HasValue && r.HasValue && l > r;
        }
        // long

        return false;
    }

    private async Task OnChange(ChangeEventArgs e)
    {
        var oldValue = Value;
        var newValue = e.Value; // object

        if (newValue is null || !TryParseValue(newValue, out var value))
            Value = default!;
        else if (EnableMinMax && Min is not null && IsLeftGreaterThanRight(Min, value)) // value < min
            Value = Min;
        else if (EnableMinMax && Max is not null && IsLeftGreaterThanRight(value, Max)) // value > max
            Value = Max;
        else
            Value = value;

        if (oldValue!.Equals(Value))
            await JSRuntime.InvokeVoidAsync("window.sincere.numberInput.setValue", Id, Value);

        await ValueChanged.InvokeAsync(Value);

        EditContext?.NotifyFieldChanged(fieldIdentifier);
    }

    private bool TryParseValue(object value, out TValue newValue)
    {
        try
        {
            if (typeof(TValue) == typeof(int?) || typeof(TValue) == typeof(int))
            {
                newValue = (TValue)Convert.ChangeType(value, typeof(int));

                return true;
            }

            if (typeof(TValue) == typeof(long?) || typeof(TValue) == typeof(long))
            {
                newValue = (TValue)Convert.ChangeType(value, typeof(long));

                return true;
            }
            newValue = default!;

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"exception: {ex.Message}");
            newValue = default!;

            return false;
        }
    }

    [Parameter]
    public bool AllowNegativeNumbers { get; set; }

    private string autoComplete => AutoComplete ? "true" : "false";

    [Parameter]
    public bool AutoComplete { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [CascadingParameter]
    private EditContext EditContext { get; set; } = default!;

    [Parameter]
    public bool EnableMinMax { get; set; }

    private string fieldCssClasses => EditContext?.FieldCssClass(fieldIdentifier) ?? "";

    [Parameter]
    public string Locale { get; set; } = "en-US";

    [Parameter]
    public TValue Max { get; set; } = default!;

    [Parameter]
    public TValue Min { get; set; } = default!;

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public double? Step { get; set; }

    [Parameter]
    public Alignment TextAlignment { get; set; } = Alignment.None;

    [Parameter]
    public TValue Value { get; set; } = default!;

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; } = default!;
}
