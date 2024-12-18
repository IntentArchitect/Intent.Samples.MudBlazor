using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;

internal class AccessTokenProvider : IAccessTokenProvider
{
    private readonly IAuthService _authService;

    public AccessTokenProvider(IAuthService authService)
    {
        _authService = authService;
    }
    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var token = await _authService.GetAccessTokenAsync();

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

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    public JwtAuthenticationStateProvider(IJSRuntime jsRuntime)
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