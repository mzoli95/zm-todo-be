
using Microsoft.EntityFrameworkCore;

namespace zm_todo.Model
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;

        public TodoRepository(TodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoDTO>> GetAllTodosAsync()
        {
            var todos = await _context.Todos
                .Include(t => t.Tags)
                .Include(t => t.Comments)
                .ToListAsync();

            return todos.Select(t => new TodoDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Completed = t.Completed,
                Deadline = t.Deadline,
                Priority = t.Priority,
                Stage = t.Stage,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                Tags = t.Tags.Select(tag => new TagDTO
                {
                    Id = tag.Id,
                    TodoId = tag.TodoId,
                    Name = tag.Name
                }).ToList(),
                Comments = t.Comments.Select(comment => new CommentDTO
                {
                    Id = comment.Id,
                    TodoId = comment.TodoId,
                    Text = comment.Text,
                    CreatedBy = comment.CreatedBy,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            }).ToList();
        }

        public async Task<TodoDTO> GetTodoByIdAsync(int id)
        {
            var todo = await _context.Todos
                .Include(t => t.Tags)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null) return null;

            return new TodoDTO
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Completed = todo.Completed,
                Deadline = todo.Deadline,
                Priority = todo.Priority,
                Stage = todo.Stage,
                CreatedBy = todo.CreatedBy,
                CreatedAt = todo.CreatedAt,
                Tags = todo.Tags.Select(tag => new TagDTO
                {
                    Id = tag.Id,
                    TodoId = tag.TodoId,
                    Name = tag.Name
                }).ToList(),
                Comments = todo.Comments.Select(comment => new CommentDTO
                {
                    Id = comment.Id,
                    TodoId = comment.TodoId,
                    Text = comment.Text,
                    CreatedBy = comment.CreatedBy,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            };
        }

        public async Task AddTodoAsync(TodoDTO todoDTO)
        {
            var todo = new Todo
            {
                Title = todoDTO.Title,
                Description = todoDTO.Description,
                Completed = todoDTO.Completed,
                Deadline = todoDTO.Deadline,
                Priority = todoDTO.Priority,
                Stage = todoDTO.Stage,
                CreatedBy = todoDTO.CreatedBy,
                CreatedAt = todoDTO.CreatedAt,
                Tags = todoDTO.Tags.Select(tagDTO => new Tag
                {
                    TodoId = tagDTO.TodoId,
                    Name = tagDTO.Name
                }).ToList(),
                Comments = todoDTO.Comments.Select(commentDTO => new Comment
                {
                    TodoId = commentDTO.TodoId,
                    Text = commentDTO.Text,
                    CreatedBy = commentDTO.CreatedBy,
                    CreatedAt = commentDTO.CreatedAt
                }).ToList()
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTodoAsync(TodoDTO todoDTO)
        {
            var todo = await _context.Todos
                .Include(t => t.Tags)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == todoDTO.Id);

            if (todo == null) return;

            todo.Title = todoDTO.Title;
            todo.Description = todoDTO.Description;
            todo.Completed = todoDTO.Completed;
            todo.Deadline = todoDTO.Deadline;
            todo.Priority = todoDTO.Priority;
            todo.Stage = todoDTO.Stage;
            todo.CreatedBy = todoDTO.CreatedBy;
            todo.CreatedAt = todoDTO.CreatedAt;

            // Update Tags
            todo.Tags.Clear();
            todo.Tags.AddRange(todoDTO.Tags.Select(tagDTO => new Tag
            {
                TodoId = tagDTO.TodoId,
                Name = tagDTO.Name
            }));

            // Update Comments
            todo.Comments.Clear();
            todo.Comments.AddRange(todoDTO.Comments.Select(commentDTO => new Comment
            {
                TodoId = commentDTO.TodoId,
                Text = commentDTO.Text,
                CreatedBy = commentDTO.CreatedBy,
                CreatedAt = commentDTO.CreatedAt
            }));

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null) return;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }
}
