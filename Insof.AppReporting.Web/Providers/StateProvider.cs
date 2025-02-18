using System;
using System.Threading.Tasks;
using Insof.AppReporting.Web.Models.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Insof.AppReporting.Web.Providers;

public interface IStateProvider
{
    Task SetIdentityAsync(PersistentIdentity? identity);
    Task<PersistentIdentity?> GetIdentityAsync();
}

public sealed class StateProvider : IStateProvider
{
    private const string UserStateKey = nameof(UserStateKey);
    
    private readonly ProtectedSessionStorage _protectedSessionStorage;
    private readonly ProtectedLocalStorage _protectedLocalStorage;

    public StateProvider(ProtectedSessionStorage protectedSessionStorage,
        ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedSessionStorage = protectedSessionStorage;
        _protectedLocalStorage = protectedLocalStorage;
    }

    public async Task SetIdentityAsync(PersistentIdentity? identity)
    {
        if (identity is null)
        {
            await _protectedSessionStorage.DeleteAsync(UserStateKey);
            await LocalDeleteAsync(UserStateKey);
        }
        else
        {
            await _protectedSessionStorage.SetAsync(UserStateKey, identity);
            await LocalSetAsync(UserStateKey, identity);
        }
    }

    public async Task<PersistentIdentity?> GetIdentityAsync()
    {
        var sessionResult = await _protectedSessionStorage.GetAsync<PersistentIdentity>(UserStateKey);
        if (sessionResult is { Success: true, Value: not null })
        {
            return sessionResult.Value;
        }

        var localResult = await LocalGetAsync<PersistentIdentity>(UserStateKey);

        var result = localResult.Success ? localResult.Value : null;

        return result?.Expiration <= DateTime.Now ? null : result;
    }

    private async ValueTask LocalSetAsync(string key, object value)
    {
        try
        {
            await _protectedLocalStorage.SetAsync(key, value);
        }
        catch (InvalidOperationException)
        {
            // ignore, this is due to attempted JavaScript interop during server rendering
        }
    }

    private async ValueTask<ProtectedStorageResult<T>> LocalGetAsync<T>(string key)
    {
        try
        {
            var result =  await _protectedLocalStorage.GetAsync<T>(key);
            return new ProtectedStorageResult<T>(result.Success, result.Value);
        }
        catch (InvalidOperationException)
        {
            // ignore, this is due to attempted JavaScript interop during server rendering
            return ProtectedStorageResult<T>.Failed;
        }
    }

    private async ValueTask LocalDeleteAsync(string key)
    {
        try
        {
            await _protectedLocalStorage.DeleteAsync(key);
        }
        catch (InvalidOperationException)
        {
            // ignore, this is due to attempted JavaScript interop during server rendering
        }
    }
}