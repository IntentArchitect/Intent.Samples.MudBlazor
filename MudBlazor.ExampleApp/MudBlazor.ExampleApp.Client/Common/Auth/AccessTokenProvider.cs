using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.JwtAuth.AccessTokenProvider", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Common.Auth;

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