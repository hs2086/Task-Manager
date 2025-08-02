using ToDoApp.Models;

namespace ToDoApp.Repository;

public interface ITaskRepository
{
    List<TaskItem> GetTasks(string userId);
    void Add(TaskItem task);
    void DeleteAll(string userId);
    void Update(TaskItem task);
    TaskItem GetTaskByID(int id);
    void Delete(int id);
    bool Toggle(int id);
    TasksStatistics GetTasksStatistics(string id);
}