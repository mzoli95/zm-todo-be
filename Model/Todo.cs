using System;
using System.Collections.Generic;
using static zm_todo.Model.TodoEnums;

namespace zm_todo.Model
{
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


}
