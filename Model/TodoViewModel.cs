using static zm_todo.Model.TodoEnums;
using System;
using System.Collections.Generic;

namespace zm_todo.Model
{
    public class TodoViewModel
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
        public EmailAddressViewModel Owned { get; set; }
        public List<TagViewModel> Tags { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<TodoAssignedToEmailAddressViewModel> AssignedTo { get; set; } = new List<TodoAssignedToEmailAddressViewModel>();
    }

    public class TagViewModel
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Name { get; set; }
    }

    public class CommentViewModel
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class EmailViewModel
    {
        public List<EmailAddressViewModel> Emails { get; set; }
    }

    public class EmailAddressViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }

    public class TodoAssignedToEmailAddressViewModel
    {
        public int Id { get; set; }

        public int TodoId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }

    }
}
