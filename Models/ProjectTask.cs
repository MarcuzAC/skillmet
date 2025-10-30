using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Gantt chart properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public int DurationDays { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.NotStarted;
        public double Progress { get; set; } = 0;

        // Effort tracking
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; } = 0;
        public int RemainingHours { get; set; }

        // Dependencies
        public int? PredecessorTaskId { get; set; }

        // Foreign keys
        public int ProjectId { get; set; }
        public int? AssignedToId { get; set; }
        public int StageId { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Employee AssignedTo { get; set; }
        public Stage Stage { get; set; }
        public ProjectTask PredecessorTask { get; set; }
        public ICollection<ProjectTask> SuccessorTasks { get; set; } = new List<ProjectTask>();
    }

    public enum ProjectTaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold,
        Blocked
    }

    public enum TaskPriority
    {
        Critical,
        High,
        Medium,
        Low
    }
}