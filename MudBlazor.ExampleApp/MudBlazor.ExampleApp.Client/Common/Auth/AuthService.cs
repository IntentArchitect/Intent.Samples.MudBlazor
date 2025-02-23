using System.Net.Http.Headers;
using System.Net.Http.Json;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.ExampleApp.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.JwtAuth.AuthServiceImplementation", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Common.Auth;

public class AuthService : IAuthService
{
    private const string AuthTokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";

    private readonly HttpClient _httpClient;
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;
    private readonly Timer _tokenCheckTimer;

    public AuthService(HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            IJSRuntime jsRuntime,
            NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = (JwtAuthenticationStateProvider)authenticationStateProvider;
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;

        _tokenCheckTimer = new Timer(async _ =>
        {
            var isLoggedIn = await IsLoggedIn();
            if (!isLoggedIn)
            {
                return;
            }
            var token = await GetAccessTokenAsync();
            if (token == null)
            {
                await Logout();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Check every minute
    }

    public async Task Register(string username, string password)
    {
        var loginRequest = new { Email = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/Account/Register", loginRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task<bool> Login(string username, string password)
    {
        var loginRequest = new { Email = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/Account/Login", loginRequest);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        await StoreTokens(result.AuthenticationToken, result.RefreshToken);

        _authenticationStateProvider.NotifyUserAuthentication(result.AuthenticationToken);

        return true;
    }
    public async Task<string?> GetAccessTokenAsync()
    {
        var accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthTokenKey);

        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        if (IsTokenAboutToExpire(accessToken))
        {
            accessToken = await RefreshAccessTokenAsync();
        }

        return accessToken;
    }

    private async Task<string> RefreshAccessTokenAsync()
    {
        var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);

        if (string.IsNullOrEmpty(refreshToken))
            return null;

        var refreshRequest = new { RefreshToken = refreshToken };
        var response = await _httpClient.PostAsJsonAsync("api/Account/Refresh", refreshRequest);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await StoreTokens(result.AuthenticationToken, result.RefreshToken);
            _authenticationStateProvider.NotifyUserAuthentication(result.AuthenticationToken);
            return result.AuthenticationToken;
        }

        // On failure, logout
        await Logout();
        return null;
    }

    public async Task<bool> IsLoggedIn()
    {
        var accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthTokenKey);
        return !string.IsNullOrEmpty(accessToken);
    }

    private async Task StoreTokens(string accessToken, string refreshToken)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AuthTokenKey, accessToken);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
    }

    private bool IsTokenAboutToExpire(string token)
    {
        var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(token);
        return jwt.ValidTo < DateTime.UtcNow.AddMinutes(-2); // Check if expiring within two minute
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AuthTokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        _authenticationStateProvider.NotifyUserLogout();
        _navigationManager.NavigateTo($"Auth/Login?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}", forceLoad: true);
        await _httpClient.PostAsJsonAsync("api/Account/Logout", new object());
    }

    private class TokenResponse
    {
        public string AuthenticationToken { get; set; }
        public string RefreshToken { get; set; }
    }
}