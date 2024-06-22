using zm_todo.Validation;

namespace zm_todo.Model
{
    public class TodoDTO
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
        public List<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
    }

    public class TagDTO
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Name { get; set; }
    }

    public class CommentDTO
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
