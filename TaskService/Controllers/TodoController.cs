using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskService.Entities;
using TaskService.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {

        private TodoDb TodoDb { get; set; }

        public TasksController(TodoDb taskDb)
        {
            TodoDb = taskDb;
        }

        // GET: api/Tasks/list/:UserId
        [HttpGet("list/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskList>>> GetTasks(string userId)
        {
            try
            {
                List<TaskList> tasks = await TodoDb.GetTaskLists(userId);
                return Ok(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }

        // POST api/Tasks/create
        [HttpPost("create/{userId}")]
        public async Task<ActionResult<TaskList>> CreateTask(string userId, TaskListCreate task)
        {
            var taskList = new TaskList(task.Name);
            try
            {
                await TodoDb.CreateTaskList(userId, taskList);
                return Ok(taskList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }

        // PUT api/Tasks/5
        [HttpPut("update/{userId}/{taskId}")]
        public async Task<ActionResult<TaskList>> PutTask(string userId, string taskId, TaskListUpdate taskUpdate)
        {
            var taskList = new TaskList()
            {
                Id = taskId,
                Name = taskUpdate.Name,
                Progress = taskUpdate.Progress,
                Size = taskUpdate.Size
            };
            try
            {
                await TodoDb.UpdateTaskList(userId, taskList);
                return Ok(taskList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }

        // DELETE api/Tasks/5
        [HttpDelete("delete/{userId}/{taskId}")]
        public async Task<ActionResult<bool>> Delete(string userId, string taskId)
        {
            var taskList = new TaskList()
            {
                Id = taskId
            };
            try
            {
                await TodoDb.DeleteTaskList(userId, taskList);
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        // GET: api/Tasks/list/:UserId/:TaskId
        [HttpGet("list/{userId}/{taskId}")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos(string userId, string taskId)
        {
            try
            {
                List<Todo> todos = await TodoDb.GetTodoLists(userId, taskId);
                return Ok(todos);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        // POST api/Tasks/create/:UserId/:TaskId
        [HttpPost("create/{userId}/{taskId}")]
        public async Task<ActionResult<Todo>> CreateTodo(string userId, string taskId, TodoCreate todo)
        {
            try
            {
                var Todo = await TodoDb.CreateTodo(userId, taskId, todo);
                return Ok(Todo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        // PUT api/Tasks/update/:UserId/:TaskId/:TodoId
        [HttpPut("update/{userId}/{taskId}/{todoId}")]
        public async Task<ActionResult<Todo>> PutTodo(string userId, string taskId, int todoId, TodoCreate todo)
        {
            var Todo = new Todo()
            {
                Id = todoId,
                Text = todo.Text,
                IsDone = todo.IsDone
            };
            try
            {
                await TodoDb.UpdateTodo(userId, taskId, Todo);
                return Ok(Todo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        // DELETE api/Tasks/delete/:UserId/:TaskId/:TodoId
        [HttpDelete("delete/{userId}/{taskId}/{todoId}")]
        public async Task<ActionResult<bool>> Delete(string userId, string taskId, int todoId)
        {
            var Todo = new Todo()
            {
                Id = todoId,
                Text = ""
            };
            try
            {
                await TodoDb.DeleteTodo(userId, taskId, Todo);
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        // POST api/Tasks/:UserId
        [HttpPost("{userId}")]
        public async Task<ActionResult<bool>> Post(string userId)
        {
            try
            {
                await TodoDb.CreateUserNode(userId);
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        // DELETE api/Tasks/:UserId
        [HttpDelete("{userId}")]
        public async Task<ActionResult<bool>> Delete(string userId)
        {
            try
            {
                await TodoDb.DeleteUserNode(userId);
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
    }
}
