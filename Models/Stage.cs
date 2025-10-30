using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class Stage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
        public int Order { get; set; } = 1;
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public double Progress { get; set; } = 0;
        public bool IsMilestone { get; set; } = false;

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}