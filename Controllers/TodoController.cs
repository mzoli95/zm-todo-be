using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using zm_todo.Logger;
using zm_todo.Model;
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
        [Route("List", Name = "GetTodoList")]
        public IActionResult GetTodoList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] bool isDescending = false, [FromQuery] string search = "")
        {
            if (HttpContext.Items["User"] is not FirebaseToken user)
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
            var todoList = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(todo => new TodoViewModel
            {
                Id = todo.Id,
                Title = todo.Title,
                Completed = todo.Completed,
                CreatedAt = todo.CreatedAt,
                Deadline = todo.Deadline,
                Description = todo.Description,
                Priority = todo.Priority,
                Stage = todo.Stage,
                CreatedBy = todo.CreatedBy
            }).ToList();

            var response = new
            {
                TotalRecords = totalRecords,
                Todos = todoList
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetAll", Name = "GetAllTodo")]
        public IActionResult GetAllTodo()
        {
            var todoList = _todoContext.Todos
            .Include(todo => todo.Tags)
            .Include(todo => todo.Comments)
            .ToList();
            return Ok(todoList);
        }

        [HttpPost]
        [Route("CreateTodo", Name ="AddTodos")]
        public IActionResult AddTodos(TodoDTO request)
        {
            var domainModelTodo = new Todo
            {
                Id = request.Id,
                //Comments = request.Comments.Select(commentDTO => new Comment
                //{
                //    Id = commentDTO.Id,
                //    TodoId = commentDTO.TodoId,
                //    Text = commentDTO.Text,
                //    CreatedBy = commentDTO.CreatedBy,
                //    CreatedAt = commentDTO.CreatedAt
                //}).ToList(),
                CreatedAt = request.CreatedAt,
                CreatedBy = request.CreatedBy,
                Deadline = request.Deadline,
                Description = request.Description,
                Priority = request.Priority,
                Stage = request.Stage,
                // Tags = request.Tags.Select(tagDTO => new Tag
                //{
                //    Id = tagDTO.Id,
                //    TodoId = tagDTO.TodoId,
                //    Name = tagDTO.Name
                //}).ToList(),
                Title = request.Title
            };

            _todoContext.Todos.Add(domainModelTodo);
            _todoContext.SaveChanges();
            return Ok(domainModelTodo);
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

        //[HttpGet]
        //[Route("All", Name = "GetTodos")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult<IEnumerable<TodoDTO>> GetTodos()
        //{
        //    var todos = TodoRepository.Todos.Select(s => new TodoDTO()
        //    {
        //        Id = s.Id,
        //        Comments = s.Comments,
        //        Title = s.Title,
        //        Completed = s.Completed,
        //        CreatedAt = s.CreatedAt,
        //        CreatedBy = s.CreatedBy,
        //        Deadline = s.Deadline,
        //        Description = s.Description,
        //        Priority = s.Priority,
        //        Stage = s.Stage,
        //        Tags = s.Tags
        //    });

        //    return Ok(todos);
        //}



        //[HttpGet]
        //[Route("{id:int}", Name = "GetTodoById")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]

        //public ActionResult<TodoDTO> GetTodoById(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest();

        //    var todo = TodoRepository.Todos.Where(n => n.Id.Equals(id)).FirstOrDefault();

        //    if (todo == null)
        //        return NotFound($"Todo item with id: {id} not found");

        //    var todoItemDTO = new TodoDTO
        //    {
        //        Id = todo.Id,
        //        Comments = todo.Comments,
        //       Completed = todo.Completed,
        //       CreatedAt = todo.CreatedAt,
        //       CreatedBy = todo.CreatedBy,  
        //       Deadline = todo.Deadline,    
        //       Description = todo.Description,
        //       Priority = todo.Priority,
        //        Stage = todo.Stage,
        //        Tags = todo.Tags
        //    };
        //    return Ok(todoItemDTO);

        //}


        //[HttpDelete("{id:int}", Name = "TodoDeleteById")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //public ActionResult<bool> TodoDeleteById(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest();

        //    var todo = TodoRepository.Todos.Where(n => n.Id.Equals(id)).FirstOrDefault();

        //    if (todo == null)
        //        return NotFound($"The todo item with id: {id} not found");

        //    TodoRepository.Todos.Remove(todo);

        //    return Ok(true);
        //}

        //[HttpPost]
        //[Route("Create")]
        ////api/student/create
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public ActionResult<StudentDTO> CreateStudent([FromBody] StudentDTO model)
        //{
        //    if (model == null)
        //        return BadRequest();

        //    int newId = CollegeRepository.Students.LastOrDefault().Id + 1;
        //    Student student = new Student
        //    {
        //        Id = newId,
        //        StudentName = model.StudentName,
        //        Address = model.Address,
        //        Emaill = model.Email
        //    };
        //    CollegeRepository.Students.Add(student);
        //    model.Id = student.Id;
        //    return CreatedAtRoute("GetStudentById", new { id = model.Id }, model);
        //}

        //[HttpPut]
        //[Route("Update")]
        ////api/student/update
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public ActionResult<StudentDTO> UpdateStudent([FromBody] StudentDTO model)
        //{
        //    if (model == null | model.Id <= 0)
        //        return BadRequest();

        //    var existingStudent = CollegeRepository.Students.Where(s => s.Id == model.Id).FirstOrDefault();

        //    if (existingStudent == null)
        //        return NotFound();

        //    existingStudent.StudentName = model.StudentName;
        //    existingStudent.Address = model.Address;
        //    existingStudent.Emaill = model.Email;

        //    return NoContent();
        //}

        //[HttpPatch]
        //[Route("{id:int}/UpdatePartial")]
        ////api/student/1/updatepartial
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public ActionResult<StudentDTO> UpdatePartialStudent(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        //{
        //    if (patchDocument == null | id <= 0)
        //        return BadRequest();

        //    var existingStudent = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();

        //    if (existingStudent == null)
        //        return NotFound();

        //    var studentDTO = new StudentDTO
        //    {
        //        Id = existingStudent.Id,
        //        Address = existingStudent.Address,
        //        Email = existingStudent.Emaill,
        //        StudentName = existingStudent.StudentName,
        //    };

        //    patchDocument.ApplyTo(studentDTO, ModelState);
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    existingStudent.StudentName = studentDTO.StudentName;
        //    existingStudent.Address = studentDTO.Address;
        //    existingStudent.Emaill = studentDTO.Email;

        //    return NoContent();


        //}

    }
}
