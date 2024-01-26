using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Front.Entities
{
    public class Todo
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public required string Text { get; set; }
        public bool IsDone { get; set; }
    }
    
    public class TaskList
    {
        [MaxLength(36)] 
        public string Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        public int Progress { get; set; } = 0;
        public int Size { get; set; } = 0;

        public TaskList()
        {
            Id = "00000000-0000-0000-0000-000000000000";
            Name = "Untilted";
        }
        
        public TaskList(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
    
    public class TodoCreate
    {
        [MaxLength(255)]
        public string Text { get; set; }
        public bool IsDone { get; set; }
    }
    
    public class TaskListCreate
    {
        [MaxLength(30)]
        public string Name { get; set; }
    }
    
    public class TaskListUpdate
    {
        [MaxLength(30)]
        public string Name { get; set; }
        public int Progress { get; set; }
        public int Size { get; set; }
    }
}
