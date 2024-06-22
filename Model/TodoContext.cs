using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace zm_todo.Model
{
    public class TodoContext : DbContext
    {

        public DbSet<Todo> Todos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Todo>()
                .HasMany(t => t.Tags)
                .WithOne(tag => tag.Todo)
                .HasForeignKey(tag => tag.TodoId);

            modelBuilder.Entity<Todo>()
                .HasMany(t => t.Comments)
                .WithOne(comment => comment.Todo)
                .HasForeignKey(comment => comment.TodoId);
        }
    }
}
