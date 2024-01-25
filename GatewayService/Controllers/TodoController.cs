using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        HttpClient client;
        public TaskController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5002/");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyTaskListAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Tasks/list/{userId}");
            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<List<TaskList>>();
                //var tasks = JsonSerializer.Deserialize<Entities.Task[]>(json);
                return Ok(tasks);
            }
            else
            {
                return BadRequest("Failed to fetch tasks");
            }

        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateTaskList(TaskListCreate task)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Tasks/create/{userId}", task);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<TaskList>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("CreateTask failed");
            }
        }
        
        [Authorize]
        [HttpPut("update/{taskId}")]
        public async Task<ActionResult> UpdateTask(string taskId, TaskListUpdate task)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Tasks/update/{userId}/{taskId}", task);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<TaskList>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("UpdateTask failed");
            }

        }

        [Authorize]
        [HttpDelete("delete/{taskId}")]
        public async Task<ActionResult> DeleteTask(string taskId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            HttpResponseMessage response = await client.DeleteAsync($"api/Tasks/delete/{userId}/{taskId}");

            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadFromJsonAsync<bool>();
                if (str == true)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return BadRequest("UpdateTask failed");
            }
        }
        
        [Authorize]
        [HttpGet("list/{taskId}")]
        public async Task<ActionResult> GetTodos(string taskId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Tasks/list/{userId}/{taskId}");

            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var task = await response.Content.ReadFromJsonAsync<List<Todo>>();
                return Ok(task);
            }
            else
            {
                return BadRequest("GetTodos failed");
            }
        }
        
        [Authorize]
        [HttpPost("create/{taskId}")]
        public async Task<ActionResult> CreateTodo(string taskId, TodoCreate todo)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Tasks/create/{userId}/{taskId}", todo);

            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var task = await response.Content.ReadFromJsonAsync<Todo>();
                return Ok(task);
            }
            else
            {
                return BadRequest("CreateTodo failed");
            }
        }
        
        [Authorize]
        [HttpPut("update/{taskId}/{todoId}")]
        public async Task<ActionResult> UpdateTodo(string taskId, int todoId, TodoCreate todo)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();
            
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Tasks/update/{userId}/{taskId}/{todoId}", todo);

            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var task = await response.Content.ReadFromJsonAsync<Todo>();
                return Ok(task);
            }
            else
            {
                return BadRequest("UpdateTodo failed");
            }
        }
        
        [Authorize]
        [HttpDelete("delete/{taskId}/{todoId}")]
        public async Task<ActionResult> Delete(string taskId, int todoId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return Unauthorized();
            
            HttpResponseMessage response = await client.DeleteAsync($"api/Tasks/delete/{userId}/{taskId}/{todoId}");

            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadFromJsonAsync<bool>();
                if (str == true)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return BadRequest("DeleteTodo failed");
            }
        }
    }
}
