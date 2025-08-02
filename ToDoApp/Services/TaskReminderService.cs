using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    public class TaskReminderService : BackgroundService
    {
        private readonly ILogger<TaskReminderService> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        public TaskReminderService(
            ILogger<TaskReminderService> logger,
            IServiceProvider services,
            IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task Reminder Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        
                        // Get tasks due in exactly 1 hour that haven't had reminders sent
                        var dueSoon = await dbContext.TaskItems
                            .Where(t => !t.ReminderSent && 
                                       t.DueDate <= DateTime.Now.AddHours(1) &&
                                       !t.IsCompleted &&
                                       t.DueDate > DateTime.Now)
                            .ToListAsync(stoppingToken);

                        foreach (var task in dueSoon)
                        {
                            var user = dbContext.Users.FirstOrDefault(u => u.Id == task.UserId);
                            if (user != null)
                            {
                                string emailUser = user.Email ?? "";
                                string userName = user.UserName ?? "";
                                await SendReminderEmail(task, emailUser, userName);
                                task.ReminderSent = true;
                                await dbContext.SaveChangesAsync(stoppingToken);
                            }
                        }
                    }
                    
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Task Reminder Service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task SendReminderEmail(TaskItem task, string userEmail, string userName)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EMAIL_CONFIGURATION");
                using var smtpClient = new SmtpClient(emailConfig["HOST"], emailConfig.GetValue<int>("PORT"))
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        emailConfig["EMAIL"],
                        emailConfig["PASSWORD"])
                }; 
                var subject = $"Task Reminder: {task.Title}";
                var body = $"Hi {userName},\n\n" +
                          $"This is a reminder that your task \"{task.Title}\" is due at {task.DueDate.ToString("h:mm tt")} " +
                          $"(in about 1 hour).\n\n" +
                          $"Task details: {task.Description}\n\n" +
                          $"Thanks,\nYour Todo App Team";

                var message = new MailMessage(
                    emailConfig["EMAIL"] ?? "",
                    userEmail,
                    subject,
                    body);

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation($"Sent reminder for task {task.Id} to {userEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reminder for task {task.Id}");
                throw;
            }
        }
    }
}