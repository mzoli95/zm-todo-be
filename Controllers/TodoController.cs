using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Xml.Linq;
using zm_todo.Logger;
using zm_todo.Model;
using static zm_todo.Model.TodoEnums;
using ILogger = zm_todo.Logger.ILogger;


namespace zm_todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly TodoContext _todoContext;

        public TodoController(ILogger<TodoController> logger, TodoContext todoContext)
        {
            _logger = logger;
            _todoContext = todoContext;
        }

        [HttpGet]
        public ActionResult Index()
        {
            _logger.LogInformation("Index method started");
            return Ok();
        }

        [HttpGet]
        [Route("{id}", Name = "GetTodoById")]
        public IActionResult GetTodoById(int id)
        {
            if (!(HttpContext.Items["User"] is FirebaseToken user))
            {
                return Unauthorized();
            }

            var query = _todoContext.Todos.AsQueryable();

            var todo = _todoContext.Todos
                 .Include(t => t.Owned)
                 .Include(t => t.Tags)
                 .Include(t => t.Comments)
                 .Include(t => t.AssignedTo)
                 .Where(t => t.Id == id)
                 .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Completed = todo.Completed,
                    CreatedAt = todo.CreatedAt,
                    Deadline = todo.Deadline,
                    Description = todo.Description,
                    Priority = todo.Priority.ToString(),
                    Stage = todo.Stage.ToString(),
                    CreatedBy = todo.CreatedBy,
                    Owned = new EmailAddressViewModel
                    {
                        Id = todo.Owned.Id,
                        Email = todo.Owned.Email,
                        DisplayName = todo.Owned.DisplayName,
                    }
,
                    Tags = todo.Tags.Select(tag => new TagViewModel
                    {
                        Id = tag.Id,
                        TodoId = tag.TodoId,
                        Name = tag.Name.ToString()
                    }).ToList(),
                    Comments = todo.Comments.Select(comment => new CommentViewModel
                    {
                        Id = comment.Id,
                        TodoId = comment.TodoId,
                        Text = comment.Text.ToString(),
                        CreatedAt = comment.CreatedAt,
                        CreatedBy = comment.CreatedBy,
                    }).ToList(),
                    AssignedTo = todo.AssignedTo.Select(assignedTo => new TodoAssignedToEmailAddressViewModel
                    {
                        Id = assignedTo.Id,
                        TodoId = assignedTo.TodoId,
                        Email = assignedTo.Email,
                        DisplayName = assignedTo.DisplayName
                    }).ToList()

                })
                .FirstOrDefault();

            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }



        [HttpGet]
        [Route("List", Name = "GetTodoList")]
        public IActionResult GetTodoList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] bool isDescending = false, [FromQuery] string search = "")
        {
            if (!(HttpContext.Items["User"] is FirebaseToken user))
            {
                return Unauthorized();
            }

            var query = _todoContext.Todos.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(todo => todo.Title.Contains(search) || todo.Description.Contains(search));
            }

            query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, sortBy)) : query.OrderBy(e => EF.Property<object>(e, sortBy));

            var totalRecords = query.Count();

            var todoList = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Completed = todo.Completed,
                    CreatedAt = todo.CreatedAt,
                    Deadline = todo.Deadline,
                    Description = todo.Description,
                    Priority = todo.Priority.ToString(),
                    Stage = todo.Stage.ToString(),
                    CreatedBy = todo.CreatedBy,
                    Owned = new EmailAddressViewModel
                    {
                        Id = todo.Owned.Id,
                        DisplayName = todo.Owned.DisplayName,
                        Email = todo.Owned.Email,
                    },
                    Tags = todo.Tags.Select(tag => new TagViewModel
                    {
                        Id = tag.Id,
                        TodoId = tag.TodoId,
                        Name = tag.Name.ToString()
                    }).ToList(),
                    Comments = todo.Comments.Select(comment => new CommentViewModel
                    {
                        Id = comment.Id,
                        TodoId = comment.TodoId,
                        Text = comment.Text.ToString(),
                        CreatedAt = comment.CreatedAt,
                        CreatedBy = comment.CreatedBy,
                    }).ToList(),
                    AssignedTo = todo.AssignedTo.Select(assignedTo => new TodoAssignedToEmailAddressViewModel
                    {
                        Id = assignedTo.Id,
                        TodoId = assignedTo.TodoId,
                        Email = assignedTo.Email,
                        DisplayName = assignedTo.DisplayName
                    }).ToList()
                })
                .ToList();

            var response = new
            {
                TotalRecords = totalRecords,
                Todos = todoList
            };

            return Ok(response);
        }
        


        [HttpPut]
        [Route("UpdateTodo/{id}", Name = "UpdateTodo")]
        public IActionResult UpdateTodo(int id, [FromBody] TodoDTO request)
        {
            var existingTodo = _todoContext.Todos
                .Include(t => t.Tags)
                .Include(t => t.Comments)
                .Include(t => t.AssignedTo)
                .FirstOrDefault(t => t.Id == id);

            if (existingTodo == null)
            {
                return NotFound($"Todo with ID {id} not found.");
            }

            existingTodo.CreatedAt = request.CreatedAt;
            existingTodo.CreatedBy = request.CreatedBy;
            existingTodo.Deadline = request.Deadline;
            existingTodo.Description = request.Description;
            existingTodo.Priority = request.Priority;
            existingTodo.Stage = request.Stage;
            existingTodo.Owned = new EmailAddress
            {
                Id = request.Owned.Id,
                Email = request.Owned.Email,
                DisplayName = request.Owned.DisplayName,
            };
            existingTodo.Title = request.Title;

            existingTodo.Tags.Clear();
            existingTodo.Tags = request.Tags.Select(tagDTO => new Tag
            {
                TodoId = id,
                Name = (TagEnum)Enum.Parse(typeof(TagEnum), tagDTO.Name)
            }).ToList();

            existingTodo.Comments.Clear();
            existingTodo.Comments = request.Comments.Select(commentDTO => new Comment
            {
                Id = commentDTO.Id,
                TodoId = id,
                Text = commentDTO.Text,
                CreatedBy = commentDTO.CreatedBy,
                CreatedAt = commentDTO.CreatedAt
            }).ToList();
            existingTodo.AssignedTo = request.AssignedTo.Select(emailDTO => new TodoAssignedToEmailAddress
            {
                TodoId = id,
                Email = emailDTO.Email,
                DisplayName = emailDTO.DisplayName,
            }).ToList();

            _todoContext.SaveChanges();

            return Ok(existingTodo);
        }


        [HttpPost]
        [Route("CreateTodo", Name ="AddTodos")]
        public IActionResult AddTodos(TodoDTO request)
        {
            var domainModelTodo = new Todo
            {
                CreatedAt = request.CreatedAt,
                CreatedBy = request.CreatedBy,
                Deadline = request.Deadline,
                Description = request.Description,
                Priority = request.Priority,
                Completed = request.Completed,
                Stage = request.Stage,
                Owned = new EmailAddress { 
                    Id = request.Owned.Id,
                    Email = request.Owned.Email,
                    DisplayName = request.Owned.DisplayName,
                },
                Title = request.Title,
                Tags = request.Tags.Select(tagDTO => new Tag
                {
                    TodoId = request.Id,
                    Name = (TagEnum)Enum.Parse(typeof(TagEnum), tagDTO.Name)
                }).ToList(),
                Comments = request.Comments.Select(commentDTO => new Comment
                {
                    TodoId = commentDTO.TodoId,
                    Text = commentDTO.Text,
                    CreatedBy = commentDTO.CreatedBy,
                    CreatedAt = commentDTO.CreatedAt
                }).ToList(),
                AssignedTo = request.AssignedTo.Select(emailDTO => new TodoAssignedToEmailAddress
                {
                    TodoId = request.Id,
                    Email = emailDTO.Email,
                    DisplayName = emailDTO.DisplayName,
                }).ToList()

        };

            var existingEmail = _todoContext.EmailAddresses.FirstOrDefault(e => e.Id == request.Owned.Id);
            domainModelTodo.Owned = existingEmail ?? new EmailAddress
            {
                Id = request.Owned.Id,
                Email = request.Owned.Email,
                DisplayName = request.Owned.DisplayName,
            };

            _todoContext.Todos.Add(domainModelTodo);
            _todoContext.SaveChanges();
            return Ok(domainModelTodo);
        }


        [HttpDelete]
        [Route("Delete/{id}/Comment/{commentId}", Name = "DeleteTodoComment")]
        public IActionResult DeleteTodoComment( int id,  int commentId)
        {
            var todo = _todoContext.Todos.Include(t => t.Comments).FirstOrDefault(t => t.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            var commentToRemove = todo.Comments.FirstOrDefault(c => c.Id == commentId);

            if (commentToRemove == null)
            {
                return NotFound();
            }

            todo.Comments.Remove(commentToRemove);
            _todoContext.SaveChanges();

            return Ok(new { message = "Comment deleted successfully" });
        }


        [HttpDelete]
        [Route("DeleteTodo/{id}", Name = "DeleteTodo")]
        public IActionResult DeleteTodo(int id)
        {
            var todo = _todoContext.Todos.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            _todoContext.Todos.Remove(todo);
            _todoContext.SaveChanges();

            return Ok(); 
        }

        [HttpPost]
        [Route("email", Name = "AddEmail")]
        [AllowAnonymous]
        public IActionResult AddEmail(EmailAddressDTO request)
        {
            var domainModelTodo = new EmailAddress
            {
               Id = request.Id,
               Email = request.Email,
               DisplayName = request.DisplayName,
               
            };

            _todoContext.EmailAddresses.Add(domainModelTodo);
            _todoContext.SaveChanges();
            return Ok(domainModelTodo);
        }

        [HttpGet]
        [Route("emailList", Name = "GetEmailList")]
        [AllowAnonymous]
        public IActionResult GetEmails(string search = "")
        {
            IQueryable<EmailAddress> query = _todoContext.EmailAddresses;

            // Ha a search paraméter üres vagy csak whitespace karakterekből áll
            if (string.IsNullOrWhiteSpace(search))
            {
                var emails = query.Select(e => new EmailAddressViewModel
                {
                    Id = e.Id,
                    Email = e.Email,
                    DisplayName = e.DisplayName
                }).ToList();

                var viewModel = new EmailViewModel
                {
                    Emails = emails
                };

                return Ok(viewModel);
            }
            else // Ha van értéke a search paraméternek
            {
                string searchTerm = search.Trim().ToLower();
                query = query.Where(e =>
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.DisplayName.ToLower().Contains(searchTerm)
                );

                var emails = query.Select(e => new EmailAddressViewModel
                {
                    Id = e.Id,
                    Email = e.Email,
                    DisplayName = e.DisplayName
                }).ToList();

                var viewModel = new EmailViewModel
                {
                    Emails = emails
                };

                return Ok(viewModel);
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmailAddress email)
        {
            try
            {
                _todoContext.EmailAddresses.Add(email);
                await _todoContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
