namespace ToDoApp.ViewModel;

public class ProfileVM
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int NumberOfTasks { get; set; }
    public int NumberOfTasksCompleted { get; set; }
    public int NumberOfTasksMissed { get; set; }
    public int NumberOfTasksPending { get; set; }
    
    public double CompletionPercentage => 
        NumberOfTasks > 0 ? (NumberOfTasksCompleted * 100.0 / NumberOfTasks) : 0;
        
    public double PendingPercentage => 
        NumberOfTasks > 0 ? (NumberOfTasksPending * 100.0 / NumberOfTasks) : 0;
        
    public double MissedPercentage => 
        NumberOfTasks > 0 ? (NumberOfTasksMissed * 100.0 / NumberOfTasks) : 0;
        
    public double AdjustedPendingPercentage => 
        Math.Min(PendingPercentage, 100 - CompletionPercentage - MissedPercentage);
}