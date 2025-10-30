using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public DateTime Deadline { get; set; }

        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
        public double Progress { get; set; } = 0;

        public decimal Budget { get; set; } = 0;
        public int EstimatedHours { get; set; } = 0;
        public int ActualHours { get; set; } = 0;
        public string ClientName { get; set; }

        // ADD THIS LINE ONLY
        public string AssignedToUserName { get; set; }

        // Navigation properties
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectSkill> RequiredSkills { get; set; } = new List<ProjectSkill>();
        public ICollection<Stage> Stages { get; set; } = new List<Stage>();
    }

    public enum ProjectPriority
    {
        Critical,
        High,
        Medium,
        Low
    }

    public enum ProjectStatus
    {
        Planning,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }
}