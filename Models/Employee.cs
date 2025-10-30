using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100)]
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;
        public decimal CurrentWorkload { get; set; } = 0;
        public int MaxWeeklyHours { get; set; } = 40;

        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
    }

    public enum AvailabilityStatus
    {
        Available,
        Busy,
        OnLeave,
        InMeeting,
        Training
    }
}