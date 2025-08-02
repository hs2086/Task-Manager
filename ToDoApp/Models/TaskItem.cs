using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Today;

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string UserId { get; set; }  // For user association
        [Display(Name = "Reminder Sent")]
        public bool ReminderSent { get; set; }
    }
}