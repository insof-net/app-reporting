namespace Insof.AppReporting.Web.Models.Authentication;

// This code only exists because Microsoft's ProtectedBrowserStorageResult struct has an internal constructor,
// and I need to be able to return values without actually using ProtectedBrowserStorage

public readonly struct ProtectedStorageResult<TValue>
{
    public bool Success { get; }

    public TValue? Value { get; } = default;

    internal ProtectedStorageResult(bool success, TValue? value)
    {
        Success = success;
        if (success)
            Value = value;
    }

    private ProtectedStorageResult(bool success)
    {
        Success = success;
    }

    internal static ProtectedStorageResult<TValue> Failed => new(false);
}