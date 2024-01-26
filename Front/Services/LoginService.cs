using Front.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Front.Services
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;
        private ProtectedLocalStorage _sessionStorage;

        public LoginService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<dynamic> AuthenticateUser(string username, string password, bool rememberMe = false)
        {
            var login = new UserLogin(username, password, rememberMe);

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/login", login);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JWTAndUser>();

                await _sessionStorage.SetAsync("jwt", result.Token);
                return result.User;
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

        public async Task<dynamic> RegisterUser(string username, string password,string email)
        {
            var registerInfo = new UserUpdateModel(username, email, password);

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/register", registerInfo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                return result;
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        
        public async Task<UserDTO> UpdateUser(string username, string password,string email)
        {
            var registerInfo = new UserUpdateModel(username, email, password);
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync("http://localhost:5000/api/User/update", registerInfo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                return result;
            }
            else
            {
                return null;
            }
        }
        
        public async Task<string> DeleteUser()
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            HttpResponseMessage response = await _httpClient.DeleteAsync("http://localhost:5000/api/User/delete");
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}

