using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Models;
using ToDoApp.Services;

namespace ToDoApp.Controllers;

public class HomeController : Controller
{
    private readonly TaskServices taskServices;
    private readonly EmailService emailService;
    private readonly ProfileServices profileServices;

    public HomeController(TaskServices taskServices, EmailService emailService, ProfileServices profileServices)
    {
        this.taskServices = taskServices;
        this.emailService = emailService;
        this.profileServices = profileServices;
    }

    public IActionResult Index()
    {
        if (User?.Identity?.IsAuthenticated == false)
        {
            return View("NotAuthonticated");
        }
        Claim? Id = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (Id == null) {
            return RedirectToAction("Login", "Account");
        }
        List<TaskItem> taskItems = taskServices.GetAllTasks(Id.Value);
        return View(taskItems);
    }
    [HttpGet]
    public IActionResult AddTask()
    {
        Claim? Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (Id == null) {
            return RedirectToAction("Login", "Account");
        }
        return View(new TaskItem { UserId = Id.Value});
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTask(TaskItem task)
    {
        if (task.Title == null || task.Description == null)
        {
            return View(task);
        }
        Claim? id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (id == null) {
            return RedirectToAction("Login", "Account");
        }
        ApplicationUser user = profileServices.GetUserByID(id.Value);
        task.CreatedDate = DateTime.Now;
        taskServices.Add(task);
        string message = $"Task '{task.Title}' added successfully! Due on {task.DueDate.ToString("dd MM yyyy hh:mm:ss tt")}";
        await emailService.SendEmail(user.Email ?? "", user.UserName ?? "", message, true);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
        TaskItem task = taskServices.GetTaskByID(id);
        return View("Edit", task);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(TaskItem task)
    {
        if (task.Title == null || task.Description == null)
        {
            return View("Edit", task);
        }
        taskServices.Update(task);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        taskServices.Delete(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleComplete(int id)
    {
        taskServices.Toggle(id);
        return RedirectToAction("Index");
    }

    public IActionResult DeleteAll()
    {
        Claim? id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (id == null)
        {
            return RedirectToAction("Login", "Account");
        }
        taskServices.DeleteAll(id.Value);
        return RedirectToAction("Index");
    }
}
