namespace ToDoApp.Models;

public class TasksStatistics
{
    public int NumberOfTasks { get; set; }
    public int NumberOfTasksCompleted { get; set; }
    public int NumberOfTasksMissed { get; set; }
    public int NumberOfTasksPending { get; set; }
}