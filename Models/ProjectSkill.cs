using System.ComponentModel.DataAnnotations;

namespace DepartmentalSystemAPI.Models
{
    public class ProjectSkill
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int SkillId { get; set; }

        [Range(1, 5)]
        public int RequiredProficiency { get; set; } = 3;
        public int Importance { get; set; } = 5; // 1-10 scale

        public Project Project { get; set; }
        public Skill Skill { get; set; }
    }
}