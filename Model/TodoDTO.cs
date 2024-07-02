using zm_todo.Validation;
using static zm_todo.Model.TodoEnums;
using System;
using System.Collections.Generic;

namespace zm_todo.Model
{
    public class TodoDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public DateTime Deadline { get; set; }
        public PriorityEnum Priority { get; set; }
        public StageEnum Stage { get; set; }
        public EmailAddressDTO Owned { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        public List<TodoAssignedToEmailAddressDTO> AssignedTo { get; set; } = new List<TodoAssignedToEmailAddressDTO>();
    }

    public class TagDTO
    {
        public int Id { get; set; }
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



    public class EmailAddressDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }

    public class TodoAssignedToEmailAddressDTO
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }

    }
}
