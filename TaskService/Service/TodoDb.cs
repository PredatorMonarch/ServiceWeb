using Microsoft.EntityFrameworkCore;
using TaskService.Entities;
using Neo4j.Driver;

namespace TaskService.Service
{
    public class TodoDb(string uri, string username, string password)
    {
        private readonly IDriver _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        public async Task CreateUserNode(string userId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(
                    "CREATE (u:User {id: $userId})",
                    new { userId });
                await result.ConsumeAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task DeleteUserNode(string userId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var taskLists = await GetTaskLists(userId);
                foreach (var task in taskLists)
                {
                    await DeleteTaskList(userId, task);
                }
                
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId}) DETACH DELETE u",
                    new { userId });
                await result.ConsumeAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task CreateTaskList(string userId, TaskList taskList)
        {
            var session = _driver.AsyncSession();
            try
            {
                // First, create the task list node
                var createResult = await session.RunAsync(
                    "CREATE (t:TaskList {id: $id, name: $name, progress: $progress, size: $size}) RETURN t",
                    new { id = taskList.Id, name = taskList.Name, progress = taskList.Progress, size = taskList.Size });
                await createResult.ConsumeAsync();

                // Then, create a relationship between the user node and the task list node
                var relateResult = await session.RunAsync(
                    "MATCH (u:User {id: $userId}), (t:TaskList {id: $taskListId}) CREATE (u)-[h:HAS_TASKLIST]->(t)",
                    new { userId, taskListId = taskList.Id });
                await relateResult.ConsumeAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task UpdateTaskList(string userId, TaskList taskList)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[h:HAS_TASKLIST]->(t:TaskList {id: $id}) SET t.name = $name, t.progress = $progress, t.size = $size RETURN t",
                    new { userId, id = taskList.Id, name = taskList.Name, progress = taskList.Progress, size = taskList.Size});
                await result.ConsumeAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task DeleteTaskList(string userId, TaskList taskList)
        {
            var session = _driver.AsyncSession();
            try
            {
                // First, delete the Todo nodes related to the TaskList node
                var todoLists = await GetTodoLists(userId, taskList.Id);
                
                foreach (var todo in todoLists)
                {
                    await DeleteTodo(userId, taskList.Id, todo);
                }
                
                // Then, delete the TaskList node
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[h:HAS_TASKLIST]->(t:TaskList {id: $id}) DELETE h, t",
                    new { userId, id = taskList.Id });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task<Todo> CreateTodo(string userId, string taskListId, TodoCreate todo)
        {
            var session = _driver.AsyncSession();
            try
            {
                var todos = await GetTodoLists(userId, taskListId);
                var id = (todos.Any() ? todos.Max(t => t.Id) : 0) + 1;
                // First, create the Todo node
                var createResult = await session.RunAsync(
                    "CREATE (t:Todo {id: $id, text: $text, isDone: $isDone}) RETURN t",
                    new { id = id, text = todo.Text, isDone = todo.IsDone });
                await createResult.ConsumeAsync();

                // Then, create a relationship between the task list node and the Todo node
                var relateResult = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[:HAS_TASKLIST]->(tl:TaskList {id: $taskListId}), (todo:Todo {id: $todoId}) CREATE (tl)-[:HAS_TODO]->(todo)",
                    new { userId, taskListId, todoId = id });
                await relateResult.ConsumeAsync();
                var Todo = new Todo
                {
                    Id = id,
                    Text = todo.Text,
                    IsDone = todo.IsDone
                };
                return Todo;
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task UpdateTodo(string userId, string taskListId, Todo todo)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[:HAS_TASKLIST]->(tl:TaskList {id: $taskListId})-[r:HAS_TODO]->(todo:Todo {id: $id}) SET todo.text = $text, todo.isDone = $isDone RETURN todo",
                    new { userId, taskListId, id = todo.Id, text = todo.Text, isDone = todo.IsDone });
                await result.ConsumeAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task DeleteTodo(string userId, string taskListId, Todo todo)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[:HAS_TASKLIST]->(tl:TaskList {id: $taskListId})-[r:HAS_TODO]->(todo:Todo {id: $id}) DELETE r, todo",
                    new { userId, taskListId, id = todo.Id });
                await result.ConsumeAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task<List<TaskList>> GetTaskLists(string userId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[:HAS_TASKLIST]->(t:TaskList) RETURN t",
                    new { userId });
                var taskLists = await result.ToListAsync(r => new TaskList
                {
                    Id = r["t"].As<Dictionary<string, object>>()["id"].As<string>(),
                    Name = r["t"].As<Dictionary<string, object>>()["name"].As<string>(),
                    Progress = r["t"].As<Dictionary<string, object>>()["progress"].As<int>(),
                    Size = r["t"].As<Dictionary<string, object>>()["size"].As<int>()
                });
                return taskLists;
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        public async Task<List<Todo>> GetTodoLists(string userId, string taskListId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(
                    "MATCH (u:User {id: $userId})-[:HAS_TASKLIST]->(tl:TaskList {id: $taskListId})-[r:HAS_TODO]->(todo:Todo) RETURN todo",
                    new { userId, taskListId });
                var todos = await result.ToListAsync(r => new Todo
                {
                    Id = r["todo"].As<Dictionary<string, object>>()["id"].As<int>(),
                    Text = r["todo"].As<Dictionary<string, object>>()["text"].As<string>(),
                    IsDone = r["todo"].As<Dictionary<string, object>>()["isDone"].As<bool>()
                });
                return todos;
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
