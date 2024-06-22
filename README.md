# Todo Application Backend

## Introduction

This project is the backend implementation of a simple Todo application, allowing for the creation, editing, and deletion of tasks, as well as management of associated labels and comments.

## Data Model

The application utilizes the following data model for storing tasks, labels, and comments:

```csharp
using System;
using System.Collections.Generic;

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
        public List<Tag> Tags { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Name { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public int TodoId { get; set; }

        public string Text { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public TodoContext(DbContextOptions options) : base(options)
        {
        }
    }
}
```

## Creating Database Migration

To create and initialize the database, the following steps need to be executed in the Program.cs file:

```csharp
builder.Services.AddDbContext<TodoContext>(options =>
{
    options.UseMySQL("Server=127.0.0.1;Port=3306;Database=zm_todo;Uid=root;Pwd=root; ");
});
```

Then, in the Package Manager Console, execute the following commands to create and apply the migration:

```
add-migration InitialCreate
update-database
```