using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
namespace WebApp.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ISessionStorageService _sessionStorageService;
        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorageService = sessionStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var email = await _sessionStorageService.GetItemAsync<string>("email");
            var level = await _sessionStorageService.GetItemAsync<string>("level");
            ClaimsIdentity identiy;
            if (email != null)
            {
                identiy = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, level),
                }, "apiauth_type");
            }
            else
            {
                identiy = new ClaimsIdentity();

            }

            var user = new ClaimsPrincipal(identiy);
            return await Task.FromResult(new AuthenticationState(user));
        }

        public void MarkUserAsAuthenticated(string emailAddress, int RoleLevel)
        {
            var identiy = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, emailAddress),
                new Claim(ClaimTypes.Role, RoleLevel.ToString()),
            }, "apiauth_type");
            var user = new ClaimsPrincipal(identiy);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        public void MarkUserAsLoggedout()
        {
            _sessionStorageService.RemoveItemAsync("email");
            _sessionStorageService.RemoveItemAsync("AccessToken");
            _sessionStorageService.RemoveItemAsync("Level");

            var identiy = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identiy);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
