using ToDoApp.Models;
using ToDoApp.Repository;
using ToDoApp.ViewModel;

namespace ToDoApp.Services;

public class ProfileServices
{
    private readonly IProfileRepository profileRepository;
    private readonly ITaskRepository taskRepository;

    public ProfileServices(IProfileRepository profileRepository, ITaskRepository taskRepository)
    {
        this.profileRepository = profileRepository;
        this.taskRepository = taskRepository;
    }

    public ApplicationUser GetUserByID(string id)
    {
        ApplicationUser? user = profileRepository.GetUserByID(id);
        if (user == null)
        {
            return new ApplicationUser();
        }
        return user;
    }
    public ProfileVM GetProfileInfo(string id)
    {
        ProfileVM profileVM = new ProfileVM();
        // Name
        // Email
        NameAndEmail nameAndEmail = profileRepository.GetNameAndEmail(id);
        profileVM.Name = nameAndEmail.Name;
        profileVM.Email = nameAndEmail.Email;
        // NumberOfTasks
        // NumberOfTasksCompleted
        // NumberOfTasksMissed
        // NumberOfTasksPending

        TasksStatistics tasksStatistics = taskRepository.GetTasksStatistics(id);
        profileVM.NumberOfTasks = tasksStatistics.NumberOfTasks;
        profileVM.NumberOfTasksCompleted = tasksStatistics.NumberOfTasksCompleted;
        profileVM.NumberOfTasksMissed = tasksStatistics.NumberOfTasksMissed;
        profileVM.NumberOfTasksPending = tasksStatistics.NumberOfTasksPending;
        return profileVM;
    }
}