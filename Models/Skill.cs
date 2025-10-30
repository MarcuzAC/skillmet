using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public SkillCategory Category { get; set; }

        // Navigation properties
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
    }

    public enum SkillCategory
    {
        Technical,
        SoftSkills,
        Management,
        Creative,
        Analytical
    }
}