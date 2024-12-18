public interface IAuthService
{
    Task Register(string username, string password);
    Task<bool> Login(string username, string password);
    Task Logout();
    Task<string?> GetAccessTokenAsync();
    Task<bool> IsLoggedIn();
}