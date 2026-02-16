using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace EventEaseApp.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string userName, string password);
        Task LogoutAsync();
        Task<string?> GetCurrentUserAsync();
        Task<bool> RegisterAsync(string userName, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly CustomAuthStateProvider _customProvider;
        private readonly IJSRuntime _js;

        // fallback demo users
        private readonly Dictionary<string, string> _demoUsers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["alice"] = "password",
            ["bob"] = "password"
        };

        private const string UserStoreKey = "userStore";

        public AuthService(AuthenticationStateProvider authStateProvider, IJSRuntime js)
        {
            _authStateProvider = authStateProvider;
            _js = js;
            _customProvider = authStateProvider as CustomAuthStateProvider
                ?? throw new InvalidOperationException("CustomAuthStateProvider must be registered.");
        }

        private async Task<Dictionary<string,string>> LoadUserStoreAsync()
        {
            try
            {
                var json = await _js.InvokeAsync<string>("localStorage.getItem", UserStoreKey);
                if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, string>(_demoUsers, StringComparer.OrdinalIgnoreCase);
                var store = JsonSerializer.Deserialize<Dictionary<string,string>>(json) ?? new();
                // merge demo users if missing
                foreach (var kv in _demoUsers) if (!store.ContainsKey(kv.Key)) store[kv.Key] = kv.Value;
                return new Dictionary<string,string>(store, StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                return new Dictionary<string, string>(_demoUsers, StringComparer.OrdinalIgnoreCase);
            }
        }

        private async Task SaveUserStoreAsync(Dictionary<string,string> store)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", UserStoreKey, JsonSerializer.Serialize(store));
        }

        public async Task<bool> LoginAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) return false;
            var store = await LoadUserStoreAsync();
            if (store.TryGetValue(userName.Trim(), out var pw) && pw == password)
            {
                await _customProvider.MarkUserAsAuthenticated(userName.Trim());
                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _customProvider.MarkUserAsLoggedOut();
        }

        public async Task<string?> GetCurrentUserAsync()
        {
            var state = await _authStateProvider.GetAuthenticationStateAsync();
            return state.User.Identity?.IsAuthenticated == true ? state.User.Identity?.Name : null;
        }

        public async Task<bool> RegisterAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) return false;
            var store = await LoadUserStoreAsync();
            if (store.ContainsKey(userName.Trim())) return false;
            store[userName.Trim()] = password;
            await SaveUserStoreAsync(store);
            // auto-login new user
            await _customProvider.MarkUserAsAuthenticated(userName.Trim());
            return true;
        }
    }
}