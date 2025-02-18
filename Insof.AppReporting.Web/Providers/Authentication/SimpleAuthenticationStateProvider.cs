using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Insof.AppReporting.Client;
using Insof.AppReporting.Client.Models;
using Insof.AppReporting.Web.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace Insof.AppReporting.Web.Providers.Authentication;

public sealed class SimpleAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IStateProvider _stateProvider;
    private readonly IUserProvider _userProvider;
    private readonly IAppReportingClient _appReportingClient;

    public SimpleAuthenticationStateProvider(IStateProvider stateProvider,
        IUserProvider userProvider, 
        IAppReportingClient appReportingClient)
    {
        _stateProvider = stateProvider;
        _userProvider = userProvider;
        _appReportingClient = appReportingClient;
    }

    public async Task<bool> Login(string username, string password)
    {
        var user = await GetUserAsync(username, password);
        var authenticationState = new AuthenticationState(user.ClaimsPrincipal);

        var identity = new PersistentIdentity
        {
            UserId = user.UserId,
            Secret = user.Secret,
            Expiration = DateTime.Now.AddDays(90)
        };

        await _stateProvider.SetIdentityAsync(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        return authenticationState.User.Identity is { IsAuthenticated: true };
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetCurrentUserAsync();
        return new AuthenticationState(user.ClaimsPrincipal);
    }

    private async Task<InsofUser> GetUserAsync(string username, string password)
    {
        return await _userProvider.AuthenticateAsync(username, password);
    }

    public async Task LogoutAsync()
    {
        await _stateProvider.SetIdentityAsync(null);
        var authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
    }

    public async Task<InsofUser> GetCurrentUserAsync()
    {
        var user = InsofUser.Empty;
        try
        {
            var persistentIdentity = await _stateProvider.GetIdentityAsync();
            if (persistentIdentity != null)
            {
                user = await _userProvider.GetFromPersistentIdentityAsync(persistentIdentity);
            }
        }
        catch (Exception ex)
        {
            _ = _appReportingClient.LogEventAsync<SimpleAuthenticationStateProvider>(SeverityLevel.Warning,
                ex.Message, null, ex);
            user = InsofUser.Empty;
        }

        return user;
    }
}
