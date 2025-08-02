using ToDoApp.Models;

namespace ToDoApp.Repository;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext context;

    public TaskRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    public List<TaskItem> GetTasks(string userId)
    {
        List<TaskItem> taskItems = context.TaskItems.Where(t => t.UserId == userId).OrderBy(t => t.DueDate).ToList();

        return taskItems;
    }
    public void Add(TaskItem task)
    {
        context.TaskItems.Add(task);
        context.SaveChanges();
    }

    public void DeleteAll(string userId)
    {
        List<TaskItem> tasks = context.TaskItems.Where(t => t.UserId == userId).ToList();
        context.TaskItems.RemoveRange(tasks);
        context.SaveChanges();
    }
    public void Update(TaskItem task)
    {
        context.TaskItems.Update(task);
        context.SaveChanges();
    }
    public TaskItem GetTaskByID(int id)
    {
        return context.TaskItems.Find(id) ?? new TaskItem();
    }

    public void Delete(int id)
    {
        TaskItem task = GetTaskByID(id);
        context.TaskItems.Remove(task);
        context.SaveChanges();
    }
    public bool Toggle(int id)
    {
        TaskItem task = GetTaskByID(id);
        if (task == null)
        {
            return false;
        }
        task.IsCompleted = !task.IsCompleted;
        try
        {
            Update(task);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public TasksStatistics GetTasksStatistics(string id)
    {
        TasksStatistics tasksStatistics = new TasksStatistics();

        tasksStatistics.NumberOfTasks = context.TaskItems.Where(t => t.UserId == id).Count();
        tasksStatistics.NumberOfTasksCompleted = context.TaskItems.Where(t => t.IsCompleted && t.UserId == id).Count();
        tasksStatistics.NumberOfTasksMissed = context.TaskItems.Where(t => t.DueDate < DateTime.Now && t.UserId == id && !t.IsCompleted).Count();
        tasksStatistics.NumberOfTasksPending = context.TaskItems.Where(t => t.DueDate > DateTime.Now && !t.IsCompleted && t.UserId == id).Count();

        return tasksStatistics;
    }
    
}