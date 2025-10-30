using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class EmployeeSkill
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int SkillId { get; set; }

        [Range(1, 5)]
        public int ProficiencyLevel { get; set; } = 1;
        public int YearsOfExperience { get; set; } = 0;
        public DateTime LastUsed { get; set; } = DateTime.UtcNow;
        public bool IsCertified { get; set; } = false;

        // Navigation properties
        public Employee Employee { get; set; }
        public Skill Skill { get; set; }
    }
}