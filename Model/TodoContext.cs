using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static zm_todo.Model.TodoEnums;

namespace zm_todo.Model
{
    public class TodoContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TodoAssignedToEmailAddress> TodoAssignedToEmailAddresses { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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


        }
    }
}
