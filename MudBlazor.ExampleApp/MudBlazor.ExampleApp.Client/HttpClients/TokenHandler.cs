using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public class TokenHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public TokenHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Handle token refresh or logout
        }

        return response;
    }
}

public interface IAuthService
{
    Task Register(string username, string password);
    Task<bool> Login(string username, string password);
    Task Logout();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = (CustomAuthenticationStateProvider) authenticationStateProvider;
        _jsRuntime = jsRuntime;
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
        Console.Write(result);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.AuthenticationToken);

        _authenticationStateProvider.NotifyUserAuthentication(result.AuthenticationToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AuthenticationToken);

        return true;
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        _authenticationStateProvider.NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private class TokenResponse
    {
        public string AuthenticationToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}

internal class AccessTokenProvider : IAccessTokenProvider
{
    private readonly IJSRuntime _jsRuntime;

    public AccessTokenProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (string.IsNullOrEmpty(token))
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "auth/login");
        }
        var accessToken = new AccessToken
        {
            Expires = DateTimeOffset.MaxValue,
            Value = token
        };

        var result = new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null);

        return result;
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
    {
        return await RequestAccessToken();
    }
}

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;

    public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(FixBase64String(payload));
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private string FixBase64String(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: return base64 + "==";
            case 3: return base64 + "=";
            default: return base64;
        }
    }
}