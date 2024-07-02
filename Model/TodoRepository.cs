using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using zm_todo.Validation;
using static zm_todo.Model.TodoEnums;

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
                .Include(t => t.AssignedTo)
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
                Owned = new EmailAddressDTO
                {
                    Id = t.Owned.Id,
                    Email = t.Owned.Email,
                    DisplayName = t.Owned.DisplayName,
                },
                Tags = t.Tags.Select(tag => new TagDTO
                {
                    Id = tag.Id,
                    Name = tag.Name.ToString()
                }).ToList(),
                Comments = t.Comments.Select(comment => new CommentDTO
                {
                    Id = comment.Id,
                    TodoId = comment.TodoId,
                    Text = comment.Text,
                    CreatedBy = comment.CreatedBy,
                    CreatedAt = comment.CreatedAt
                }).ToList(),
                AssignedTo = new List<TodoAssignedToEmailAddressDTO>(t.AssignedTo.Select(at => new TodoAssignedToEmailAddressDTO
                {
                    Id = at.Id,
                    TodoId = at.TodoId,
                    DisplayName= at.DisplayName,
                    Email = at.Email,
                }))
            }).ToList();
        }

        public async Task<TodoDTO> GetTodoByIdAsync(int id)
        {
            var todo = await _context.Todos
                .Include(t => t.Tags)
                .Include(t => t.Comments)
                .Include(t => t.AssignedTo)
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
                Owned = new EmailAddressDTO
                {
                    Id = todo.Owned.Id,
                    Email = todo.Owned.Email,
                    DisplayName = todo.Owned.DisplayName,
                },
                Tags = todo.Tags.Select(tag => new TagDTO
                {
                    Id = tag.Id,
                    Name = tag.Name.ToString()
                }).ToList(),
                Comments = todo.Comments.Select(comment => new CommentDTO
                {
                    Id = comment.Id,
                    TodoId = comment.TodoId,
                    Text = comment.Text,
                    CreatedBy = comment.CreatedBy,
                    CreatedAt = comment.CreatedAt
                }).ToList(),
                AssignedTo = new List<TodoAssignedToEmailAddressDTO>(todo.AssignedTo.Select(at => new TodoAssignedToEmailAddressDTO
                {
                    Id = at.Id,
                    TodoId = at.TodoId,
                    Email = at.Email,
                    DisplayName = at.DisplayName
                }))
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
                Owned = new EmailAddress
                {
                    Email = todoDTO.Owned.Email,
                    DisplayName = todoDTO.Owned.DisplayName
                },
                Tags = todoDTO.Tags.Select(tagDTO => new Tag
                {
                    Name = (TagEnum)Enum.Parse(typeof(TagEnum), tagDTO.Name)
                }).ToList(),
                Comments = todoDTO.Comments.Select(commentDTO => new Comment
                {
                    TodoId = commentDTO.TodoId,
                    Text = commentDTO.Text,
                    CreatedBy = commentDTO.CreatedBy,
                    CreatedAt = commentDTO.CreatedAt
                }).ToList(),
                AssignedTo = new List<TodoAssignedToEmailAddress>(todoDTO.AssignedTo.Select(a => new TodoAssignedToEmailAddress
                {
                    TodoId = a.TodoId,
                    Email = a.Email,
                    DisplayName = a.DisplayName
                }))
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTodoAsync(TodoDTO todoDTO)
        {
            var todo = await _context.Todos
                .Include(t => t.Tags)
                .Include(t => t.Comments)
                .Include(t => t.AssignedTo)
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
            todo.Owned.Email = todoDTO.Owned.Email;
            todo.Owned.DisplayName = todoDTO.Owned.DisplayName;

            // Clear and re-add Tags
            todo.Tags.Clear();
            todo.Tags.AddRange(todoDTO.Tags.Select(tagDTO => new Tag
            {
                Name = (TagEnum)Enum.Parse(typeof(TagEnum), tagDTO.Name)
            }));

            // Clear and re-add Comments
            todo.Comments.Clear();
            todo.Comments.AddRange(todoDTO.Comments.Select(commentDto => new Comment
            {
                TodoId = commentDto.TodoId,
                Text = commentDto.Text,
                CreatedBy = commentDto.CreatedBy,
                CreatedAt = commentDto.CreatedAt
            }));

            // Clear and re-add AssignedTo
            todo.AssignedTo.Clear();
            todo.AssignedTo.AddRange(todoDTO.AssignedTo.Select(a => new TodoAssignedToEmailAddress
            {
                TodoId = a.TodoId,
                Email = a.Email,
                DisplayName = a.DisplayName,
               
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
