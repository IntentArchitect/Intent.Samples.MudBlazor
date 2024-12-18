using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;


public class AuthService : IAuthService
{
    private const string AuthTokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";

    private readonly HttpClient _httpClient;
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime _jsRuntime;
    private readonly Timer _tokenCheckTimer;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = (JwtAuthenticationStateProvider) authenticationStateProvider;
        _jsRuntime = jsRuntime;

        _tokenCheckTimer = new Timer(async _ =>
        {
            var token = await GetAccessTokenAsync();
            if (token == null)
            {
                _authenticationStateProvider.NotifyUserLogout();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Check every minute
    }

    public async Task Register(string username, string password)
    {
        var loginRequest = new { Email = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/Account/Register", loginRequest);
    }

    public async Task<bool> Login(string username, string password)
    {
        var loginRequest = new { Email = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/Account/Login", loginRequest);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        await StoreTokens(result.AuthenticationToken, result.RefreshToken);

        _authenticationStateProvider.NotifyUserAuthentication(result.AuthenticationToken);
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AuthenticationToken);

        return true;
    }
    public async Task<string?> GetAccessTokenAsync()
    {
        var accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthTokenKey);

        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        if (IsTokenExpired(accessToken))
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

    private bool IsTokenExpired(string token)
    {
        var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(token);
        return jwt.ValidTo < DateTime.UtcNow.AddMinutes(-1); // Check if expiring within a minute
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AuthTokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        _authenticationStateProvider.NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private class TokenResponse
    {
        public string AuthenticationToken { get; set; }
        public string RefreshToken { get; set; }
    }
}