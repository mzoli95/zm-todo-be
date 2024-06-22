
namespace zm_todo.Model
{

    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public string Stage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Tag
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Name { get; set; }
        public Todo Todo { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Todo Todo { get; set; }
    }
}
