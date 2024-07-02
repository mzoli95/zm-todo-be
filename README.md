# Todo Application Backend V1.0

## Introduction

This project is the backend implementation of a simple Todo application, allowing for the creation, editing, and deletion of tasks, as well as management of associated labels and comments. The project is under continuous development and refactoring, aiming to showcase my skills, although my expertise is more frontend-heavy.

## Data Model

## V1.0

The application utilizes the following data model for storing tasks, assigners, labels, and comments:

```csharp

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public DateTime Deadline { get; set; }
    public PriorityEnum Priority { get; set; }
    public StageEnum Stage { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public EmailAddress Owned { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<TodoAssignedToEmailAddress> AssignedTo { get; set; } = new List<TodoAssignedToEmailAddress>();
}

public class Email
{
    public List<EmailAddress> Emails { get; set; } = new List<EmailAddress>();
}

public class Tag
{
    public int Id { get; set; }
    public int TodoId { get; set; }
    public TagEnum Name { get; set; }
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

public class TodoAssignedToEmailAddress
{
    public int Id { get; set; }

    public int TodoId { get; set; } 
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public Todo Todo { get; set; }

}

public class EmailAddress
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}

```

## Database Configuration

The database connection is managed by the TodoContext class, which is a descendant of the DbContext class and uses DbSet type properties to represent tables. We configure it as follows:

```csharp
using Microsoft.EntityFrameworkCore;

namespace zm_todo.Model
{
    public class TodoContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TodoAssignedToEmailAddress> TodoAssignedToEmailAddresses { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }

        public TodoContext(DbContextOptions options) : base(options)
        {
        }
    }
}

# Relations
 modelBuilder.Entity<EmailAddress>()
     .HasKey(e => e.Id);

 modelBuilder.Entity<Todo>()
     .Property(t => t.Priority)
     .HasConversion<string>();

 modelBuilder.Entity<Todo>()
     .Property(t => t.Stage)
     .HasConversion<string>();

 modelBuilder.Entity<Tag>()
     .Property(t => t.Name)
     .HasConversion(
         v => v.ToString(),
         v => (TagEnum)Enum.Parse(typeof(TagEnum), v)
     );

 modelBuilder.Entity<Todo>()
     .HasMany(t => t.Tags)
     .WithOne(tag => tag.Todo)
     .HasForeignKey(tag => tag.TodoId);

 modelBuilder.Entity<Tag>()
     .HasKey(t => t.Id);

 modelBuilder.Entity<Tag>()
     .HasIndex(t => new { t.TodoId, t.Name })
     .IsUnique();

 modelBuilder.Entity<Todo>()
     .HasMany(t => t.Comments)
     .WithOne(comment => comment.Todo)
     .HasForeignKey(comment => comment.TodoId);

 modelBuilder.Entity<TodoAssignedToEmailAddress>()
     .HasKey(tae => tae.Id);

 modelBuilder.Entity<TodoAssignedToEmailAddress>()
     .HasOne(tae => tae.Todo)
     .WithMany(t => t.AssignedTo)
     .HasForeignKey(tae => tae.TodoId);
```

## Creating Database Migration

To create and initialize the database, the following steps need to be executed in the Program.cs file:

```csharp
builder.Services.AddDbContext<TodoContext>(options =>
{
    options.UseMySQL("Server=[*];Port=[*];Database=[*];Uid=[*];Pwd=[*]; ");
});
```

Then, in the Package Manager Console, execute the following commands to create and apply the migration:

```
add-migration InitialCreate
update-database
```

# Todo API

This API provides CRUD operations for managing Todo items. Below are the details of each operation.

## CRUD Operations

### 1. Create a Todo
**Endpoint:** `POST /api/TodoController/CreateTodo`

**Description:** This operation allows you to create a new Todo item.

```csharp
[HttpPost]
[Route("CreateTodo", Name ="AddTodos")]
public IActionResult AddTodos(TodoDTO request)
{
    // Code to create a new Todo item
}
```

### 2. Read a Todo by ID
**Endpoint:** `GET /api/TodoController/{id}`

**Description:** This operation retrieves a Todo item by its ID.

```csharp
[HttpGet]
[Route("{id}", Name = "GetTodoById")]
public IActionResult GetTodoById(int id)
{
    // Code to get a Todo item by ID
}
```
### 3. Read a List of Todos
**Endpoint:** `GET /api/TodoController/List`

**Description:** This operation retrieves a paginated list of Todo items, optionally filtered by search criteria.

```csharp
[HttpGet]
[Route("List", Name = "GetTodoList")]
public IActionResult GetTodoList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] bool isDescending = false, [FromQuery] string search = "")
{
    // Code to get a list of Todo items
}
```

### 4. Update a Todo
**Endpoint:** `PUT /api/TodoController/UpdateTodo/{id}`

**Description:** This operation updates an existing Todo item by its ID.

```csharp
[HttpPut]
[Route("UpdateTodo/{id}", Name = "UpdateTodo")]
public IActionResult UpdateTodo(int id, [FromBody] TodoDTO request)
{
    // Code to update a Todo item by ID
}
```

### 5. Delete a Todo
**Endpoint:** `DELETE /api/TodoController/DeleteTodo/{id}`

**Description:** This operation deletes a Todo item by its ID.

```csharp
[HttpDelete]
[Route("DeleteTodo/{id}", Name = "DeleteTodo")]
public IActionResult DeleteTodo(int id)
{
    // Code to delete a Todo item by ID
}
```

## Additional Operations

### Add an Email Address
**Endpoint:** `POST /api/TodoController/email`

**Description:** This operation allows you to add a new email address after register.

```csharp
[HttpPost]
[Route("email", Name = "AddEmail")]
[AllowAnonymous]
public IActionResult AddEmail(EmailAddressDTO request)
{
    // Code to add a new email address
}
```

### Get a List of Email Addresses
**Endpoint:** `GET /api/TodoController/emailList`

**Description:** This operation retrieves a list of email addresses, optionally filtered by search criteria.

```csharp
[HttpGet]
[Route("emailList", Name = "GetEmailList")]
[AllowAnonymous]
public IActionResult GetEmails(string search = "")
{
    // Code to get a list of email addresses
}
```

### Delete a Comment from a Todo
**Endpoint:** `DELETE /api/TodoController/Delete/{id}/Comment/{commentId}`

**Description:** This operation deletes a comment from a specific Todo item.

```csharp
[HttpDelete]
[Route("Delete/{id}/Comment/{commentId}", Name = "DeleteTodoComment")]
public IActionResult DeleteTodoComment(int id, int commentId)
{
    // Code to delete a comment from a Todo item
}
```

## Logging
**Description:** The API uses a custom logger defined in zm_todo.Logger. Under maintenance.

```csharp
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly TodoContext _todoContext;

    public TodoController(ILogger<TodoController> logger, TodoContext todoContext)
    {
        _logger = logger;
        _todoContext = todoContext;
    }

    // Other methods...
}
```

## Database Context
The TodoContext class, defined in zm_todo.Model, is used to interact with the database.

## Security
Authentication is handled using FirebaseToken.

# How to Run
Clone the repository.
Configure the database connection in appsettings.json.
Run the migrations to set up the database.
Start the application.
