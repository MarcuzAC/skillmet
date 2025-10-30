namespace DepartmentalSystemAPI.DTOs
{
    public class StageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public double Progress { get; set; }
        public bool IsMilestone { get; set; }
        public List<ProjectTaskDto> Tasks { get; set; } = new();
    }

    public class ProjectSkillDto
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Category { get; set; }
        public int RequiredProficiency { get; set; }
        public int Importance { get; set; }
    }
}