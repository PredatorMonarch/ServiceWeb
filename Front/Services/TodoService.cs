using Front.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;

namespace Front.Services
{
    public class TodoService
    {
        private readonly HttpClient _httpClient;
        private ProtectedLocalStorage _sessionStorage;

        public TodoService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<List<TaskList>> GetAllTasks()
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/api/Task");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<TaskList>>();

                return result;
            }
            else
            {
                return [];
            }
        }

        public async Task<TaskList> CreateNewTaskList(string name)
        {
            var task = new TaskListCreate() { Name = name };

            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/Task/create", task);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TaskList>();

                return result;
            }
            else
            {
                return null;
            }
        }
        public async Task<TaskList> UpdateTaskList(TaskList taskList)
        {
            var task = new TaskListUpdate() { Name = taskList.Name, Progress = taskList.Progress, Size = taskList.Size };

            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            var id = taskList.Id;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"http://localhost:5000/api/Task/update/{id}", task);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TaskList>();

                return result;
            }
            else
            {
                return null;
            }
        }
        
        public async Task<bool> DeleteTaskList(string id)
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.DeleteAsync($"http://localhost:5000/api/Task/delete/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            
            return false;
        }
        
        
        public async Task<List<Todo>> GetAllTodos(string id)
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5000/api/Task/list/{id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<Todo>>();

                return result;
            }
            else
            {
                return [];
            }
        }
        
        public async Task<Todo> CreateNewTodoItem(string id, TodoCreate todo)
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"http://localhost:5000/api/Task/create/{id}", todo);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Todo>();

                return result;
            }
            else
            {
                return null;
            }
        }
        
        public async Task<Todo> UpdateTodoItem(string id, Todo todo)
        {
            var _todo = new TodoCreate() { Text = todo.Text, IsDone = todo.IsDone };
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"http://localhost:5000/api/Task/update/{id}/{todo.Id}", _todo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Todo>();

                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteTodoItem(string id, Todo todo)
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
            HttpResponseMessage response =
                await _httpClient.DeleteAsync($"http://localhost:5000/api/Task/delete/{id}/{todo.Id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }
    }
}

