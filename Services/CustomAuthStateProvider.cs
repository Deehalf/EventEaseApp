using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace EventEaseApp.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private const string StorageKey = "authUser";
        private readonly IJSRuntime _js;

        public CustomAuthStateProvider(IJSRuntime js)
        {
            _js = js;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var user = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
                if (!string.IsNullOrWhiteSpace(user))
                {
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user) }, "apiauth");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
            catch { /* ignore JS errors */ }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task MarkUserAsAuthenticated(string userName)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "apiauth");
            var principal = new ClaimsPrincipal(identity);
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, userName);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }
    }
}