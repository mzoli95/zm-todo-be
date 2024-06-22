namespace zm_todo.Logger
{
    public class LogToDB : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Log to db");
        }
    }
}
