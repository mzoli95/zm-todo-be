namespace zm_todo.Model
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoDTO>> GetAllTodosAsync();
        Task<TodoDTO> GetTodoByIdAsync(int id);
        Task AddTodoAsync(TodoDTO todo);
        Task UpdateTodoAsync(TodoDTO todo);
        Task DeleteTodoAsync(int id);
    }
}
