using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using ToDoApp.Models;
using ToDoApp.Repository;

namespace ToDoApp.Services;

public class TaskServices
{
    private readonly ITaskRepository taskRepository;

    public TaskServices(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public List<TaskItem> GetAllTasks(string userId)
    {
        if (userId == null)
        {
            throw new ArgumentNullException("user id can not be null");
        }
        List<TaskItem> taskItems = taskRepository.GetTasks(userId);
        return taskItems;
    }
    public void Add(TaskItem task)
    {
        if (task == null)
        {
            throw new ArgumentNullException("Task can not be null");
        }
        try
        {
            taskRepository.Add(task);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }
    public void DeleteAll(string userId)
    {
        if (userId == null)
        {
            throw new ArgumentNullException("user id can not be null");
        }
        taskRepository.DeleteAll(userId);
    }

    public void Update(TaskItem task)
    {
        if (task == null)
        {
            throw new ArgumentNullException("Task can not be null");
        }
        taskRepository.Update(task);
    }
    public TaskItem GetTaskByID(int id)
    {
        return taskRepository.GetTaskByID(id);
    }
    public void Delete(int id)
    {
        if (id == 0)
        {
            throw new ArgumentOutOfRangeException("Id can not be null");
        }
        taskRepository.Delete(id);
    }
    public void Toggle(int id)
    {
        if (id == 0)
        {
            throw new ArgumentOutOfRangeException("Id can not be null");
        }
        bool toggled = taskRepository.Toggle(id);
        if (!toggled)
        {
            throw new InvalidOperationException("Can't toggle this task");
        }
    }
}